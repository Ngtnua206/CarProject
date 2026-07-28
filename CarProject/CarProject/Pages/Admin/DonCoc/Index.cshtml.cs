using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.DonCoc;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    private readonly INotificationService _notif;
    public List<DonDatCoc> DonCocList { get; set; } = new();
    public List<ChiNhanhShowroom> ChiNhanhList { get; set; } = new();

    public IndexModel(AppDbContext db, IActivityLogService log, INotificationService notif)
    {
        _db = db;
        _log = log;
        _notif = notif;
    }

    public async Task OnGetAsync()
    {
        DonCocList = await _db.DonDatCoc
            .Include(d => d.KhachHang)
            .Include(d => d.PhienBan).ThenInclude(p => p.DongXe)
            .Include(d => d.ChiNhanh)
            .OrderByDescending(d => d.NgayTaoDon)
            .ToListAsync();
        ChiNhanhList = await _db.ChiNhanhShowroom.ToListAsync();
        await _log.LogAsync("Admin Xem danh sách đơn cọc");
    }

    public async Task<IActionResult> OnPostApproveAsync(int maDonCoc, string maChiNhanhMoi, string? lichHenNgay, string? lichHenGio)
    {
        var don = await _db.DonDatCoc.Include(d => d.KhachHang).FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
        if (don == null) return NotFound();

        don.TrangThaiDonHang = "Đã duyệt";
        don.MaQuanLyDuyet = User.GetJwtUserName();

        if (!string.IsNullOrEmpty(maChiNhanhMoi))
            don.MaChiNhanh = maChiNhanhMoi;

        await _db.SaveChangesAsync();

        if (don.MaKhachHang != null)
        {
            var msg = $"Đơn cọc #{maDonCoc} đã được duyệt.";
            if (!string.IsNullOrEmpty(don.MaChiNhanh))
            {
                var cn = await _db.ChiNhanhShowroom.FindAsync(don.MaChiNhanh);
                if (cn != null) msg += $" Showroom: {cn.TenChiNhanh}.";
            }
            await _notif.SendAsync(don.MaKhachHang, "Đơn cọc đã được duyệt", msg, "/Orders/DepositResult?maDonCoc=" + maDonCoc);
        }

        if (!string.IsNullOrEmpty(lichHenNgay) && !string.IsNullOrEmpty(lichHenGio) && don.MaKhachHang != null)
        {
var lichHen = new LichHenLaiThu
                {
                    MaKhachHang = don.MaKhachHang,
                    MaDong = don.PhienBan?.MaDong ?? 0,
                    MaChiNhanh = don.MaChiNhanh ?? maChiNhanhMoi ?? "",
                    HoTenNguoiLai = don.HoTen ?? "",
                    SoDienThoai = don.SoDienThoai ?? "",
                    SoBangLaiXe = "",
                    NgayHen = DateTime.Parse(lichHenNgay),
                    GioHen = lichHenGio,
                    TrangThai = "Chờ xác nhận",
                    YKienKhachHang = ""
                };
            _db.LichHenLaiThu.Add(lichHen);
            await _db.SaveChangesAsync();

            await _notif.SendAsync(don.MaKhachHang, "Lịch hẹn đã được tạo",
                $"Lịch hẹn lái thử vào {lichHenNgay} lúc {lichHenGio} tại showroom. Vui lòng đến đúng hẹn.",
                "/TestDrive");
        }

        await _log.LogAsync($"Admin duyệt đơn cọc #{maDonCoc}");
        TempData["Success"] = $"Đã duyệt đơn cọc #{maDonCoc}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int maDonCoc)
    {
        var don = await _db.DonDatCoc.FindAsync(maDonCoc);
        if (don == null) return NotFound();

        don.TrangThaiDonHang = "Đã hủy";
        don.MaQuanLyDuyet = User.GetJwtUserName();
        await _db.SaveChangesAsync();

        if (don.MaKhachHang != null)
        {
            await _notif.SendAsync(don.MaKhachHang, "Đơn cọc bị từ chối",
                $"Đơn cọc #{maDonCoc} đã bị từ chối. Vui lòng liên hệ showroom để biết chi tiết.",
                "");
        }

        await _log.LogAsync($"Admin từ chối đơn cọc #{maDonCoc}");
        TempData["Error"] = $"Đã từ chối đơn cọc #{maDonCoc}.";
        return RedirectToPage();
    }
}