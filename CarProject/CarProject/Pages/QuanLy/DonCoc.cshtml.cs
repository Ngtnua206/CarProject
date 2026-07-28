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
            $"Đơn cọc #{maDonCoc} đã được xác nhận. Vui lòng đến showroom để hoàn tất thủ tục.", "/Orders/DepositForm?maDonCoc=" + maDonCoc);
        await _log.LogAsync($"QL xác nhận đơn cọc #{maDonCoc}");

        SuccessMessage = "Đã xác nhận đơn cọc.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc.FindAsync(maDonCoc);
        if (don == null) return NotFound();

        don.TrangThaiDonHang = "Đã hủy";
        await _db.SaveChangesAsync();

        await _notif.SendAsync(don.MaKhachHang!, "Đơn cọc đã bị hủy",
            $"Đơn cọc #{maDonCoc} đã bị hủy. Vui lòng liên hệ showroom để biết thêm chi tiết.", "");
        await _log.LogAsync($"QL hủy đơn cọc #{maDonCoc}");

        SuccessMessage = "Đã hủy đơn cọc.";
        return RedirectToPage();
    }
}