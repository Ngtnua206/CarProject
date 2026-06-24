using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using System.Diagnostics;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

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

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<CarProject.Services.IActivityLogService, CarProject.Services.ActivityLogService>();

    // Google OAuth
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        options.CallbackPath = "/Account/GoogleCallback";
        options.SaveTokens = true;
    });

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(connectionString))
    {
        builder.Services.AddDbContext<CarProject.Data.AppDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    builder.Host.UseSerilog();

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

    app.MapDefaultEndpoints();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
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
        var isLoggedIn = context.Session.GetInt32("UserId").HasValue;

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

        // /Orders/* -> cần đăng nhập (bất kỳ role nào)
        if (path.StartsWith("/Orders", StringComparison.OrdinalIgnoreCase) && !isLoggedIn)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }

        await next(context);
    });

    app.MapStaticAssets();
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
