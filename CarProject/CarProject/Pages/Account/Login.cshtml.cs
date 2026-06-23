using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Services;

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

    public LoginModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
        {
            ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu.";
            await _log.LogAsync("Đăng nhập thất bại", $"Tài khoản rỗng");
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

        // Lưu vào session (Simple auth, không dùng Identity)
        HttpContext.Session.SetInt32("UserId", user.MaTaiKhoan);
        HttpContext.Session.SetString("UserName", user.TenDangNhap);
        HttpContext.Session.SetString("UserRole", user.VaiTro);

        // Nếu RememberMe, lưu cookie
        if (RememberMe)
        {
            Response.Cookies.Append("UserId", user.MaTaiKhoan.ToString(), 
                new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.UtcNow.AddDays(30) });
        }

        await _log.LogAsync("Đăng nhập thành công", $"Tài khoản \"{user.TenDangNhap}\" vai trò {user.VaiTro}");

        // Admin -> /Admin, user thường -> /Index
        if (user.VaiTro == "Admin")
            return RedirectToPage("/Admin/Index");

        return RedirectToPage("/Index");
    }

    public void OnGet()
    {
        // Check if already logged in
        if (HttpContext.Session.GetInt32("UserId").HasValue)
        {
            RedirectToPage("/Index");
        }
    }
}
