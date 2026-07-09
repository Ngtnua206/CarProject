using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;
using System.Security.Claims;

namespace CarProject.Pages.Account;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public string TenDangNhap { get; set; }

    [BindProperty]
    public string MatKhau { get; set; }

    [BindProperty]
    public bool RememberMe { get; set; }

    public string ErrorMessage { get; set; }
    public bool GoogleEnabled { get; set; }
    public string LoaiThongBao { get; set; }

    public LoginModel(AppDbContext db, IActivityLogService log, IConfiguration config)
    {
        _db = db;
        _log = log;
        GoogleEnabled = !string.IsNullOrEmpty(config["Authentication:Google:ClientId"]);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
        {
            ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu.";
            await _log.LogAsync("Đăng nhập thất bại", "Tài khoản rỗng");
            return Page();
        }

        var user = await _db.TaiKhoan
            .FirstOrDefaultAsync(t => t.TenDangNhap == TenDangNhap && t.MatKhau == MatKhau);

        if (user == null)
        {
            ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            await _log.LogAsync("Đăng nhập thất bại", $"Tài khoản \"{TenDangNhap}\" không đúng");
            return Page();
        }

        if (user.TrangThai == "Banned")
        {
            ErrorMessage = "Tài khoản đã bị khóa.";
            return Page();
        }

        if (user.TrangThai != "Hoạt động" && user.TrangThai != "Active")
        {
            ErrorMessage = "Tài khoản đã bị khóa.";
            return Page();
        }

        if (!user.DaXacNhanEmail && !string.IsNullOrEmpty(user.MaXacNhan))
        {
            ErrorMessage = "Tài khoản chưa xác nhận email. Vui lòng kiểm tra hộp thư.";
            return Page();
        }

        SetSession(user);
        await _log.LogAsync("Đăng nhập thành công", $"Tài khoản \"{user.TenDangNhap}\" vai trò {user.VaiTro}");

        if (user.VaiTro == "Admin")
            return RedirectToPage("/Admin/Index");

        if (user.VaiTro == "Quản Lý")
            return RedirectToPage("/QuanLy/Dashboard");

        return RedirectToPage("/Index");
    }

    public IActionResult OnGet()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            return RedirectToPage("/Index");

        if (Request.Query.ContainsKey("daCoTaiKhoan"))
        {
            LoaiThongBao = "daCoTaiKhoan";
            if (Request.Query.TryGetValue("email", out var e))
                TenDangNhap = e;
        }

        if (Request.Query.ContainsKey("dangKyThanhCong"))
            LoaiThongBao = "dangKyThanhCong";

        return Page();
    }

    public IActionResult OnGetGoogleLogin()
    {
        if (!GoogleEnabled)
            return RedirectToPage();
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Page("/Account/Login", "GoogleCallback")
        };
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> OnGetGoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            ErrorMessage = "Đăng nhập Google thất bại.";
            return Page();
        }

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value ?? email;

        if (string.IsNullOrEmpty(email))
        {
            ErrorMessage = "Không thể lấy thông tin email từ Google.";
            return Page();
        }

        var user = await _db.TaiKhoan.FirstOrDefaultAsync(t => t.TenDangNhap == email);
        var isNewUser = false;
        if (user == null)
        {
            user = new TaiKhoan
            {
                TenDangNhap = email,
                MatKhau = "",
                VaiTro = "User",
                TrangThai = "Active",
                Email = email,
                TenHienThi = name
            };
            _db.TaiKhoan.Add(user);
            await _db.SaveChangesAsync();
            // Set default name "User{MaTaiKhoan}" — TenHienThi already has Google name, user can change later
            isNewUser = true;
        }

        if (user.TrangThai == "Banned")
        {
            ErrorMessage = "Tài khoản đã bị khóa.";
            return Page();
        }

        if (user.TrangThai != "Hoạt động" && user.TrangThai != "Active")
        {
            ErrorMessage = "Tài khoản đã bị khóa.";
            return Page();
        }

        if (!user.DaXacNhanEmail && !string.IsNullOrEmpty(user.MaXacNhan))
        {
            ErrorMessage = "Tài khoản chưa xác nhận email. Vui lòng kiểm tra hộp thư.";
            return Page();
        }

        SetSession(user);
        await _log.LogAsync("Đăng nhập Google", $"Tài khoản \"{email}\"");

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (user.VaiTro == "Admin")
            return RedirectToPage("/Admin/Index");

        if (user.VaiTro == "Quản Lý")
            return RedirectToPage("/QuanLy/Dashboard");

        if (isNewUser)
            return RedirectToPage("/Profile", new { firstTime = true });

        return RedirectToPage("/Index");
    }

    private void SetSession(TaiKhoan user)
    {
        HttpContext.Session.SetString("UserName", user.TenDangNhap);
        HttpContext.Session.SetString("UserRole", user.VaiTro);
        HttpContext.Session.SetString("TenHienThi", user.TenHienThi ?? user.TenDangNhap);
        HttpContext.Session.SetString("AvatarUrl", user.AvatarUrl ?? "");
        HttpContext.Session.SetString("UserEmail", user.Email ?? user.TenDangNhap);
        HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);

        var jwt = HttpContext.RequestServices.GetRequiredService<CarProject.Services.IJwtService>();
        HttpContext.Session.SetString("JwtToken", jwt.GenerateToken(user));
    }
}
