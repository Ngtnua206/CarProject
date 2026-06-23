using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;

namespace CarProject.Pages.Account;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public string TenDangNhap { get; set; }

    [BindProperty]
    public string MatKhau { get; set; }

    [BindProperty]
    public bool RememberMe { get; set; }

    public string ErrorMessage { get; set; }

    public LoginModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
        {
            ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu.";
            return Page();
        }

        var user = await _db.TaiKhoan
            .FirstOrDefaultAsync(t => t.TenDangNhap == TenDangNhap && t.MatKhau == MatKhau);

        if (user == null)
        {
            ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
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
