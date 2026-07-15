using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Serilog;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

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

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Allow larger uploads from browsers / IDE-run profiles (set to 100 MB)
    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
        options.ValueLengthLimit = int.MaxValue;
        options.ValueCountLimit = int.MaxValue;
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<CarProject.Services.IActivityLogService, CarProject.Services.ActivityLogService>();
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

    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();

    // Authorization middleware: phân quyền theo đường dẫn
    app.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value ?? "";
        var role = context.Session.GetString("UserRole");
        var isLoggedIn = !string.IsNullOrEmpty(context.Session.GetString("UserName"));

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
        if ((path.StartsWith("/Orders", StringComparison.OrdinalIgnoreCase) ||
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
            var userId = ctx.Session.GetString("UserName");
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
                ctx.Session.SetString("AvatarUrl", user.AvatarUrl);
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

            // Update session if active
            if (!string.IsNullOrEmpty(ctx.Session.GetString("UserName")))
                ctx.Session.SetString("AvatarUrl", user.AvatarUrl);

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
