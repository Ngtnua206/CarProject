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

        if (user.TrangThai != "Active")
        {
            ErrorMessage = "Tài khoản đã bị khóa.";
            return Page();
        }

        SetSession(user);

        await _log.LogAsync("Đăng nhập thành công", $"Tài khoản \"{user.TenDangNhap}\" vai trò {user.VaiTro}");

        if (user.VaiTro == "Admin")
            return RedirectToPage("/Admin/Index");

        return RedirectToPage("/Index");
    }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserId").HasValue)
            return RedirectToPage("/Index");
        return Page();
    }

    public IActionResult OnGetGoogleLogin()
    {
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

        // Tìm hoặc tạo tài khoản
        var user = await _db.TaiKhoan.FirstOrDefaultAsync(t => t.TenDangNhap == email);
        if (user == null)
        {
            user = new TaiKhoan
            {
                TenDangNhap = email,
                MatKhau = "", // login bằng Google, không cần mật khẩu
                VaiTro = "User",
                TrangThai = "Active"
            };
            _db.TaiKhoan.Add(user);
            await _db.SaveChangesAsync();
        }

        if (user.TrangThai != "Active")
        {
            ErrorMessage = "Tài khoản đã bị khóa.";
            return Page();
        }

        SetSession(user);
        await _log.LogAsync("Đăng nhập Google", $"Tài khoản \"{email}\"");

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (user.VaiTro == "Admin")
            return RedirectToPage("/Admin/Index");

        return RedirectToPage("/Index");
    }

    private void SetSession(TaiKhoan user)
    {
        HttpContext.Session.SetInt32("UserId", user.MaTaiKhoan);
        HttpContext.Session.SetString("UserName", user.TenDangNhap);
        HttpContext.Session.SetString("UserRole", user.VaiTro);
    }
}
