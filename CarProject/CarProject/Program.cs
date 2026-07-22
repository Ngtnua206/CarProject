using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Serilog;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using CarProject.Services;
using CarProject.Data;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

// Global handlers to capture otherwise-silent crashes when running under debugger/IDE
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    try
    {
        var ex = e.ExceptionObject as Exception;
        var msg = $"UNHANDLED: {ex?.GetType().Name}: {ex?.Message}\n{ex?.StackTrace}";
        var path = Path.Combine(Path.GetTempPath(), "app_unhandled.log");
        System.IO.File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}\n\n");
        Log.Fatal(ex, "Unhandled exception");
    }
    catch { }
};

TaskScheduler.UnobservedTaskException += (s, e) =>
{
    try
    {
        var ex = e.Exception;
        var path = Path.Combine(Path.GetTempPath(), "app_unobserved_task.log");
        System.IO.File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n\n");
        Log.Error(ex, "Unobserved task exception");
    }
    catch { }
};

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddServiceDefaults();

    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    builder.Services.AddDistributedMemoryCache();

    // Allow larger uploads from browsers / IDE-run profiles (set to 100 MB)
    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
        options.ValueLengthLimit = int.MaxValue;
        options.ValueCountLimit = int.MaxValue;
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<CarProject.Services.IActivityLogService, CarProject.Services.ActivityLogService>();
    builder.Services.AddScoped<CarProject.Services.ICartService, CarProject.Services.CartService>();
    builder.Services.AddScoped<CarProject.Services.IJwtService, CarProject.Services.JwtService>();
    builder.Services.AddScoped<CarProject.Services.IPasswordService, CarProject.Services.PasswordService>();
    builder.Services.Configure<CarProject.Services.SmtpSettings>(builder.Configuration.GetSection("Smtp"));
    builder.Services.AddScoped<CarProject.Services.IEmailService, CarProject.Services.EmailService>();

    // JWT config
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "CarProjectSuperSecretKey2024@MustBe32CharsLong!";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CarProject";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CarProject";

    // Google OAuth (only if configured)
    var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
    var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    var useGoogle = !string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret);

    var authBuilder = builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = useGoogle ? GoogleDefaults.AuthenticationScheme
            : Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

    if (useGoogle)
    {
        authBuilder.AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
            options.CallbackPath = "/Account/GoogleCallback";
            options.SaveTokens = true;
        });
    }

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(connectionString))
    {
        builder.Services.AddDbContext<CarProject.Data.AppDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    builder.Host.UseSerilog();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
        // Increase max request body size to allow large JSON payloads or large uploads
        options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
    });

    var app = builder.Build();

    // Seed database on startup
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<CarProject.Data.AppDbContext>();
            context.Database.Migrate();
            CarProject.Data.DbInitializer.SeedData(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error seeding database");
        }
    }

    // Force UTF-8 charset on all text responses
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            var ct = context.Response.ContentType;
            if (ct != null && ct.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
                && ct.IndexOf("charset", StringComparison.OrdinalIgnoreCase) < 0)
            {
                context.Response.ContentType = ct + "; charset=utf-8";
            }
            return Task.CompletedTask;
        });
        await next();
    });

    app.MapDefaultEndpoints();

    if (app.Environment.IsDevelopment())
    {
        // Show detailed exceptions when running from Visual Studio / development profile
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseRouting();

    // Request logging middleware
    app.Use(async (context, next) =>
    {
        var sw = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;
        Log.Information("--> {Method} {Path}", method, path);
        await next(context);
        sw.Stop();
        Log.Information("<-- {Method} {Path} => {StatusCode} ({Elapsed}ms)", method, path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    });

    // JWT cookie auth middleware (replaces session)
    app.Use(async (context, next) =>
    {
        var token = context.GetJwtFromCookie();
        if (!string.IsNullOrEmpty(token))
        {
            var jwt = context.RequestServices.GetRequiredService<CarProject.Services.IJwtService>();
            var principal = jwt.ValidateToken(token);
            if (principal != null)
                context.User = principal;
        }
        await next(context);
    });

    app.UseAuthentication();
    app.UseAuthorization();

    // Authorization middleware: phân quyền theo đường dẫn
    app.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value ?? "";
        var role = context.User.GetJwtRole();
        var isLoggedIn = context.User.IsJwtLoggedIn();

        // /Admin/* -> chỉ Admin
        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
        {
            if (!isLoggedIn)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }
            if (role != "Admin")
            {
                context.Response.StatusCode = 403;
                return;
            }
        }

        // /Orders/*, /Profile, /TestDrive, /QuanLy/* -> cần đăng nhập
        if (((path.StartsWith("/Orders", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("/Orders/DepositForm", StringComparison.OrdinalIgnoreCase)) ||
             path.Equals("/Profile", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/TestDrive", StringComparison.OrdinalIgnoreCase) ||
             path.StartsWith("/TestDrive", StringComparison.OrdinalIgnoreCase) ||
             path.StartsWith("/QuanLy", StringComparison.OrdinalIgnoreCase))
            && !isLoggedIn)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }

        await next(context);
    });

    // Prevent browser caching of avatar images
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.Value?.StartsWith("/uploads/avatars") == true && !context.Response.HasStarted)
        {
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
        }
        await next();
    });

    app.MapStaticAssets();
    // Minimal API for avatar upload (bypasses Razor Pages pipeline entirely)
    app.MapPost("/api/upload-avatar", async (HttpContext ctx, IWebHostEnvironment env) =>
    {
        try
        {
            var userId = ctx.User.GetJwtUserName();
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            string body;
            using (var reader = new StreamReader(ctx.Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            var payload = JsonSerializer.Deserialize<JsonElement>(body);
            var dataUrl = payload.GetProperty("avatarBase64").GetString();
            if (string.IsNullOrEmpty(dataUrl))
                return Results.BadRequest(new { error = "Missing image data" });

            var commaIdx = dataUrl.IndexOf(',');
            if (commaIdx < 0)
                return Results.BadRequest(new { error = "Invalid data URL" });

            var base64 = dataUrl.Substring(commaIdx + 1);
            var bytes = Convert.FromBase64String(base64);

            var webRoot = env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot");
            webRoot = Path.GetFullPath(webRoot);
            var uploadsDir = Path.Combine(webRoot, "uploads", "avatars");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"{userId}.jpg";
            var filePath = Path.Combine(uploadsDir, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);

            var db = ctx.RequestServices.GetRequiredService<CarProject.Data.AppDbContext>();
            var user = await db.TaiKhoan.FirstOrDefaultAsync(t => t.TenDangNhap == userId);
            if (user != null)
            {
                user.AvatarUrl = $"/uploads/avatars/{fileName}";
                await db.SaveChangesAsync();
                var jwtSvc = ctx.RequestServices.GetRequiredService<CarProject.Services.IJwtService>();
                ctx.SetJwtCookie(jwtSvc.GenerateToken(user));
            }

            var log = ctx.RequestServices.GetRequiredService<CarProject.Services.IActivityLogService>();
            await log.LogAsync("Cập nhật ảnh đại diện");

            return Results.Ok(new { success = true, url = $"/uploads/avatars/{fileName}" });
        }
        catch (Exception ex)
        {
            var logPath = Path.Combine(Path.GetTempPath(), "avatar_error.log");
            System.IO.File.AppendAllText(logPath,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n\n");
            return Results.Ok(new { success = false, error = ex.Message });
        }
    });

    // Minimal API for admin avatar upload (session-independent, username from body)
    app.MapPost("/api/upload-avatar-admin", async (HttpContext ctx, IWebHostEnvironment env) =>
    {
        try
        {
            string body;
            using (var reader = new StreamReader(ctx.Request.Body))
                body = await reader.ReadToEndAsync();

            var payload = JsonSerializer.Deserialize<JsonElement>(body);
            var dataUrl = payload.GetProperty("avatarBase64").GetString();
            var userName = payload.GetProperty("userName").GetString();

            if (string.IsNullOrEmpty(dataUrl) || string.IsNullOrEmpty(userName))
                return Results.BadRequest(new { success = false, error = "Missing data" });

            // Verify user exists
            var db = ctx.RequestServices.GetRequiredService<CarProject.Data.AppDbContext>();
            var user = await db.TaiKhoan.FindAsync(userName);
            if (user == null)
                return Results.Ok(new { success = false, error = "User not found" });

            var commaIdx = dataUrl.IndexOf(',');
            if (commaIdx < 0)
                return Results.BadRequest(new { success = false, error = "Invalid data URL" });

            var base64 = dataUrl.Substring(commaIdx + 1);
            var bytes = Convert.FromBase64String(base64);

            var webRoot = env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot");
            webRoot = Path.GetFullPath(webRoot);
            var uploadsDir = Path.Combine(webRoot, "uploads", "avatars");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"{userName}.jpg";
            var filePath = Path.Combine(uploadsDir, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);

            user.AvatarUrl = $"/uploads/avatars/{fileName}";
            await db.SaveChangesAsync();

            // Regenerate JWT if active
            if (ctx.User.IsJwtLoggedIn())
            {
                var jwtSvc = ctx.RequestServices.GetRequiredService<CarProject.Services.IJwtService>();
                ctx.SetJwtCookie(jwtSvc.GenerateToken(user));
            }

            var log = ctx.RequestServices.GetRequiredService<CarProject.Services.IActivityLogService>();
            await log.LogAsync("Cập nhật ảnh đại diện (admin)");

            return Results.Ok(new { success = true, url = user.AvatarUrl });
        }
        catch (Exception ex)
        {
            return Results.Ok(new { success = false, error = ex.Message });
        }
    });

    // Minimal API for JWT login (JSON body: { tenDangNhap, matKhau })
    app.MapPost("/api/login", async (HttpContext ctx, CarProject.Services.IJwtService jwt) =>
    {
        string body;
        using (var reader = new StreamReader(ctx.Request.Body))
            body = await reader.ReadToEndAsync();

        var payload = JsonSerializer.Deserialize<JsonElement>(body);
        var tenDangNhap = payload.GetProperty("tenDangNhap").GetString() ?? "";
        var matKhau = payload.GetProperty("matKhau").GetString() ?? "";

        if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            return Results.BadRequest(new { error = "Missing credentials" });

        var db = ctx.RequestServices.GetRequiredService<CarProject.Data.AppDbContext>();
        var user = await db.TaiKhoan.FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

        if (user == null || (user.TrangThai != "Active" && user.TrangThai != "Hoạt động"))
            return Results.Unauthorized();

        var passwordService = ctx.RequestServices.GetRequiredService<CarProject.Services.IPasswordService>();
        if (!passwordService.Verify(matKhau, user.MatKhau ?? ""))
            return Results.Unauthorized();

        var token = jwt.GenerateToken(user);

        return Results.Ok(new
        {
            token,
            expiresIn = 1800,
            user = new
            {
                tenDangNhap = user.TenDangNhap,
                tenHienThi = user.TenHienThi ?? user.TenDangNhap,
                vaiTro = user.VaiTro,
                avatarUrl = user.AvatarUrl ?? ""
            }
        });
    });

    // Minimal API for cart operations
    app.MapGet("/api/cart/count", async (HttpContext ctx, ICartService cart) =>
    {
        var userName = ctx.User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName))
            return Results.Ok(new { count = 0 });
        var count = await cart.GetCartCountAsync();
        return Results.Ok(new { count });
    });

    app.MapPost("/api/cart/add", async (HttpContext ctx, ICartService cart) =>
    {
        var userName = ctx.User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName))
            return Results.Ok(new { success = false, error = "Vui lòng đăng nhập" });

        string body;
        using (var reader = new StreamReader(ctx.Request.Body))
            body = await reader.ReadToEndAsync();

        var payload = JsonSerializer.Deserialize<JsonElement>(body);
        var item = JsonSerializer.Deserialize<CartItem>(body) ?? new CartItem();

        await cart.AddToCartAsync(item);
        return Results.Ok(new { success = true });
    });

    app.MapDelete("/api/cart/remove/{id:int}", async (int id, ICartService cart) =>
    {
        await cart.RemoveFromCartAsync(id);
        return Results.Ok(new { success = true });
    });

    // Inline admin editor API endpoints
    app.MapPost("/api/admin/upload", async (HttpContext ctx, IWebHostEnvironment env) =>
    {
        var file = ctx.Request.Form.Files.FirstOrDefault();
        if (file == null) return Results.BadRequest(new { error = "No file" });
        var uploadsDir = Path.Combine(env.WebRootPath, "uploads", "admin");
        Directory.CreateDirectory(uploadsDir);
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(uploadsDir, fileName);
        using (var stream = new FileStream(path, FileMode.Create))
            await file.CopyToAsync(stream);
        return Results.Ok(new { url = $"/uploads/admin/{fileName}" });
    });

    app.MapPost("/api/admin/banner/save", async (HttpContext ctx, AppDbContext db) =>
    {
        var body = await ctx.Request.ReadFromJsonAsync<JsonElement>();
        var duongDanAnh = body.GetProperty("duongDanAnh").GetString();
        var banner = await db.QuangCaoBanner.OrderBy(b => b.ThuTuHienThi).FirstOrDefaultAsync();
        if (banner == null)
        {
            banner = new CarProject.Models.QuangCaoBanner
            {
                DuongDanAnh = duongDanAnh,
                ThuTuHienThi = 1,
                MaQuanLyCapNhat = ctx.User.GetJwtUserName() ?? "admin"
            };
            db.QuangCaoBanner.Add(banner);
        }
        else
        {
            banner.DuongDanAnh = duongDanAnh;
        }
        await db.SaveChangesAsync();
        return Results.Ok(new { success = true });
    });

    app.MapGet("/api/admin/dongxe/{id}", async (int id, AppDbContext db) =>
    {
        var dongXe = await db.DongXe.Include(d => d.HangXe).Include(d => d.PhienBanXes)
            .FirstOrDefaultAsync(d => d.MaDong == id);
        if (dongXe == null) return Results.NotFound();
        var hx = dongXe.HangXe;
        return Results.Ok(new
        {
            dongXe.MaDong,
            dongXe.TenDong,
            dongXe.KieuDang,
            dongXe.MaHang,
            dongXe.DuongDanAnh,
            brand = hx != null ? new { hx.MaHang, hx.TenHang, hx.QuocGia, hx.DuongDanLogo } : null,
            versions = dongXe.PhienBanXes?.Select(v => new
            {
                v.MaPhienBan,
                v.TenPhienBan,
                v.GiaNiemYet,
                v.MauSac,
                v.DongCo,
                v.HopSo,
                v.LoaiNhietLieu,
                v.SoLuongTrongKho,
                v.DuongDanAnh,
                v.TrangThai,
                v.MaKhuyenMai
            }).ToList()
        });
    });

    app.MapPost("/api/admin/hangxe/save", async (HttpContext ctx, AppDbContext db) =>
    {
        var body = await ctx.Request.ReadFromJsonAsync<JsonElement>();
        var maHang = body.GetProperty("maHang").GetInt32();
        var hangXe = await db.HangXe.FindAsync(maHang);
        if (hangXe == null) return Results.NotFound();
        if (body.TryGetProperty("tenHang", out var th)) hangXe.TenHang = th.GetString() ?? hangXe.TenHang;
        if (body.TryGetProperty("quocGia", out var qg)) hangXe.QuocGia = qg.GetString() ?? hangXe.QuocGia;
        if (body.TryGetProperty("duongDanLogo", out var ddl) && ddl.ValueKind != JsonValueKind.Null)
            hangXe.DuongDanLogo = ddl.GetString();
        await db.SaveChangesAsync();
        return Results.Ok(new { success = true });
    });

    app.MapPost("/api/admin/dongxe/save", async (HttpContext ctx, AppDbContext db) =>
    {
        var body = await ctx.Request.ReadFromJsonAsync<JsonElement>();
        var maDong = body.GetProperty("maDong").GetInt32();
        var dongXe = await db.DongXe.FindAsync(maDong);
        if (dongXe == null) return Results.NotFound();
        if (body.TryGetProperty("tenDong", out var td)) dongXe.TenDong = td.GetString() ?? dongXe.TenDong;
        if (body.TryGetProperty("maHang", out var mh)) dongXe.MaHang = mh.GetInt32();
        if (body.TryGetProperty("kieuDang", out var kd)) dongXe.KieuDang = kd.GetString() ?? dongXe.KieuDang;
        if (body.TryGetProperty("duongDanAnh", out var dda) && dda.ValueKind != JsonValueKind.Null)
            dongXe.DuongDanAnh = dda.GetString();
        await db.SaveChangesAsync();
        return Results.Ok(new { success = true });
    });

    app.MapPost("/api/admin/phienban/save", async (HttpContext ctx, AppDbContext db) =>
    {
        var body = await ctx.Request.ReadFromJsonAsync<JsonElement>();
        var maPhienBan = body.GetProperty("maPhienBan").GetInt32();
        var pb = await db.PhienBanXe.FindAsync(maPhienBan);
        if (pb == null) return Results.NotFound();
        if (body.TryGetProperty("tenPhienBan", out var tp)) pb.TenPhienBan = tp.GetString() ?? pb.TenPhienBan;
        if (body.TryGetProperty("giaNiemYet", out var gn)) pb.GiaNiemYet = gn.GetInt64();
        if (body.TryGetProperty("mauSac", out var ms)) pb.MauSac = ms.GetString() ?? pb.MauSac;
        if (body.TryGetProperty("dongCo", out var dc)) pb.DongCo = dc.GetString() ?? pb.DongCo;
        if (body.TryGetProperty("hopSo", out var hs)) pb.HopSo = hs.GetString() ?? pb.HopSo;
        if (body.TryGetProperty("loaiNhietLieu", out var lnl)) pb.LoaiNhietLieu = lnl.GetString() ?? pb.LoaiNhietLieu;
        if (body.TryGetProperty("soLuongTrongKho", out var slk)) pb.SoLuongTrongKho = slk.GetInt32();
        if (body.TryGetProperty("trangThai", out var tt)) pb.TrangThai = tt.GetString() ?? pb.TrangThai;
        if (body.TryGetProperty("maKhuyenMai", out var km) && km.ValueKind != JsonValueKind.Null)
            pb.MaKhuyenMai = km.GetString();
        if (body.TryGetProperty("duongDanAnh", out var dda) && dda.ValueKind != JsonValueKind.Null)
            pb.DuongDanAnh = dda.GetString();
        await db.SaveChangesAsync();
        return Results.Ok(new { success = true });
    });

    app.MapRazorPages();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
