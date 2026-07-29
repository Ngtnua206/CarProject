using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.QuanLy;

public class DonCocModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    private readonly INotificationService _notif;

    public DonCocModel(AppDbContext db, IActivityLogService log, INotificationService notif)
    {
        _db = db;
        _log = log;
        _notif = notif;
    }

    public ChiNhanhShowroom? Showroom { get; set; }
    public List<DonDatCoc> DanhSachDonCoc { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName))
            return RedirectToPage("/Account/Login");

        Showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (Showroom == null)
        {
            ErrorMessage = "Bạn chưa được phân công quản lý showroom nào.";
            return Page();
        }

        DanhSachDonCoc = await _db.DonDatCoc
            .Include(d => d.PhienBan).ThenInclude(p => p.DongXe)
            .Include(d => d.KhachHang)
            .Where(d => d.MaChiNhanh == Showroom.MaChiNhanh)
            .OrderByDescending(d => d.NgayTaoDon)
            .ToListAsync();

        return Page();
    }

    // ----- Chấp nhận đặt cọc -----
    public async Task<IActionResult> OnPostApproveAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc.FindAsync(maDonCoc);
        if (don == null) return NotFound();

        don.TrangThaiDonHang = "Đã xác nhận";
        don.MaQuanLyDuyet = userName;
        await _db.SaveChangesAsync();

        await _notif.SendAsync(don.MaKhachHang!, "Đơn cọc đã được xác nhận",
            $"Đơn cọc #{maDonCoc} đã được chấp nhận. Vui lòng đến showroom để hoàn tất thủ tục.", "");
        await _notif.SendToRoleAsync("Admin", "Đơn cọc đã được Quản Lý xác nhận",
            $"Đơn cọc #{maDonCoc} - {don.HoTen} - Vui lòng vào bán xe",
            $"/Admin/DonCoc/Index");
        await _log.LogAsync($"QL chấp nhận đơn cọc #{maDonCoc}");

        SuccessMessage = "Đã chấp nhận đơn cọc.";
        return RedirectToPage();
    }

    // ----- Không chấp nhận đặt cọc -----
    public async Task<IActionResult> OnPostRejectAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc.FindAsync(maDonCoc);
        if (don == null) return NotFound();

        don.TrangThaiDonHang = "Đã từ chối";
        don.MaQuanLyDuyet = userName;
        await _db.SaveChangesAsync();

        await _notif.SendAsync(don.MaKhachHang!, "Đơn cọc không được chấp nhận",
            $"Đơn cọc #{maDonCoc} không được chấp nhận. Tiền cọc sẽ được hoàn lại. Vui lòng liên hệ showroom.", "");
        await _log.LogAsync($"QL từ chối đơn cọc #{maDonCoc}");

        SuccessMessage = "Đã từ chối đơn cọc. Tiền cọc sẽ được hoàn lại cho khách.";
        return RedirectToPage();
    }


}
