using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public string TenDangNhap { get; set; }

    [BindProperty]
    public string MatKhau { get; set; }

    [BindProperty]
    public string XacNhanMatKhau { get; set; }

    [BindProperty]
    public string HoTen { get; set; }

    [BindProperty]
    public string SoDienThoai { get; set; }

    [BindProperty]
    public string DiaChi { get; set; }

    public string ErrorMessage { get; set; }

    public RegisterModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserId").HasValue)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(TenDangNhap) || string.IsNullOrEmpty(MatKhau))
        {
            ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu.";
            return Page();
        }

        if (MatKhau != XacNhanMatKhau)
        {
            ErrorMessage = "Mật khẩu xác nhận không khớp.";
            return Page();
        }

        if (await _db.TaiKhoan.AnyAsync(t => t.TenDangNhap == TenDangNhap))
        {
            ErrorMessage = "Tên đăng nhập đã tồn tại.";
            return Page();
        }

        var user = new TaiKhoan
        {
            TenDangNhap = TenDangNhap,
            MatKhau = MatKhau,
            VaiTro = "User",
            TrangThai = "Active"
        };
        _db.TaiKhoan.Add(user);
        await _db.SaveChangesAsync();

        var khachHang = new ChiTietKhachHang
        {
            HoTen = HoTen ?? "",
            SoDienThoai = SoDienThoai ?? "",
            DiaChi = DiaChi ?? ""
        };
        _db.ChiTietKhachHang.Add(khachHang);
        await _db.SaveChangesAsync();

        // Auto-login
        HttpContext.Session.SetInt32("UserId", user.MaTaiKhoan);
        HttpContext.Session.SetString("UserName", user.TenDangNhap);
        HttpContext.Session.SetString("UserRole", user.VaiTro);

        await _log.LogAsync("Đăng ký tài khoản", $"Tài khoản \"{TenDangNhap}\" đã tạo thành công");

        return RedirectToPage("/Index");
    }
}
