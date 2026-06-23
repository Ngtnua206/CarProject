namespace CarProject;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        // Add Session support for Login
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        // ActivityLogService
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<CarProject.Services.IActivityLogService, CarProject.Services.ActivityLogService>();

        // Register EF Core DbContext
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.Services.AddDbContext<CarProject.Data.AppDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

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
                System.Diagnostics.Debug.WriteLine($"Error seeding database: {ex.Message}");
            }
        }

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] --> {method} {path}");
            await next(context);
            sw.Stop();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] <-- {method} {path} => {context.Response.StatusCode} ({sw.ElapsedMilliseconds}ms)");
        });

        // AddSession middleware
        app.UseSession();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
