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
            .Include(d => d.ChiTiets).ThenInclude(c => c.PhienBan).ThenInclude(p => p.DongXe)
            .Include(d => d.ChiTiets).ThenInclude(c => c.ChiNhanh)
            .Include(d => d.KhachHang)
            .Where(d => d.ChiTiets.Any(c => c.MaChiNhanh == Showroom.MaChiNhanh))
            .OrderByDescending(d => d.NgayTaoDon)
            .ToListAsync();

        return Page();
    }

    // ----- Chấp nhận tiếp nhận một xe trong đơn -----
    public async Task<IActionResult> OnPostAcceptAsync(int maChiTiet)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var chiTiet = await _db.DonDatCocChiTiet
            .Include(c => c.DonDatCoc)
            .Include(c => c.PhienBan).ThenInclude(p => p.DongXe)
            .Include(c => c.ChiNhanh)
            .FirstOrDefaultAsync(c => c.MaChiTiet == maChiTiet);
        if (chiTiet == null) return NotFound();

        // Chỉ quản lý showroom nguồn của xe đó mới được xác nhận
        var showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (showroom == null || chiTiet.MaChiNhanh != showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền xác nhận xe thuộc showroom khác.";
            return RedirectToPage();
        }

        chiTiet.TrangThaiTiepNhan = "Đã tiếp nhận";
        chiTiet.NguoiPhanHoi = userName;
        chiTiet.NgayPhanHoi = DateTime.Now;
        await _db.SaveChangesAsync();

        await _log.LogAsync($"QL chấp nhận xe #{maChiTiet} của đơn #{chiTiet.MaDonCoc}");
        SuccessMessage = "Đã tiếp nhận xe trong đơn.";
        return RedirectToPage();
    }

    // ----- Từ chối tiếp nhận một xe (phải điền lý do) -----
    public async Task<IActionResult> OnPostRejectAsync(int maChiTiet, string lyDoTuChoi)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(lyDoTuChoi))
        {
            ErrorMessage = "Vui lòng nhập lý do từ chối.";
            return RedirectToPage();
        }

        var chiTiet = await _db.DonDatCocChiTiet
            .Include(c => c.DonDatCoc)
            .Include(c => c.PhienBan).ThenInclude(p => p.DongXe)
            .Include(c => c.ChiNhanh)
            .FirstOrDefaultAsync(c => c.MaChiTiet == maChiTiet);
        if (chiTiet == null) return NotFound();

        var showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (showroom == null || chiTiet.MaChiNhanh != showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền xác nhận xe thuộc showroom khác.";
            return RedirectToPage();
        }

        chiTiet.TrangThaiTiepNhan = "Từ chối";
        chiTiet.LyDoTuChoi = lyDoTuChoi.Trim();
        chiTiet.NguoiPhanHoi = userName;
        chiTiet.NgayPhanHoi = DateTime.Now;
        await _db.SaveChangesAsync();

        var tenXe = $"{chiTiet.PhienBan?.DongXe?.TenDong ?? ""} {chiTiet.PhienBan?.TenPhienBan ?? ""}".Trim();
        await _log.LogAsync($"QL từ chối xe #{maChiTiet} đơn #{chiTiet.MaDonCoc}",
            $"Lý do: {lyDoTuChoi.Trim()}");

        // Gửi thông báo tới Admin khi từ chối
        await _notif.SendToRoleAsync("Admin", "Showroom từ chối tiếp nhận xe",
            $"Showroom {showroom.TenChiNhanh} từ chối tiếp nhận xe {tenXe} (Đơn cọc #{chiTiet.MaDonCoc}). Lý do: {lyDoTuChoi.Trim()}",
            $"/Admin/DonCoc/Edit/{chiTiet.MaDonCoc}");

        if (chiTiet.DonDatCoc?.MaKhachHang != null)
        {
            await _notif.SendAsync(chiTiet.DonDatCoc.MaKhachHang, "Xe bị từ chối tiếp nhận",
                $"Xe {tenXe} trong đơn cọc #{chiTiet.MaDonCoc} bị từ chối bởi showroom {showroom.TenChiNhanh}. Lý do: {lyDoTuChoi.Trim()}",
                "");
        }

        SuccessMessage = "Đã từ chối tiếp nhận xe. Lý do đã được gửi tới Admin.";
        return RedirectToPage();
    }

    // ----- Tiếp nhận toàn bộ đơn cọc (Quản Lý showroom nhận xe xác nhận) -----
    public async Task<IActionResult> OnPostAcceptOrderAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc
            .Include(d => d.ChiTiets)
            .Include(d => d.KhachHang)
            .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
        if (don == null) return NotFound();

        var showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (showroom == null || don.MaChiNhanh != showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền tiếp nhận đơn cọc của showroom khác.";
            return RedirectToPage();
        }

        if (don.TrangThaiDonHang != "Chờ xác nhận")
        {
            ErrorMessage = "Đơn cọc không ở trạng thái cần tiếp nhận.";
            return RedirectToPage();
        }

        if (!don.ChiTiets.Any() || !don.ChiTiets.All(c => c.TrangThaiTiepNhan == "Đã tiếp nhận"))
        {
            ErrorMessage = "Chưa đủ điều kiện tiếp nhận: vẫn còn xe chưa được các showroom xác nhận.";
            return RedirectToPage();
        }

        don.TrangThaiDonHang = "Đã xác nhận";
        don.MaQuanLyDuyet = userName;
        await _db.SaveChangesAsync();

        await _notif.SendAsync(don.MaKhachHang!, "Đơn cọc đã được tiếp nhận",
            $"Đơn cọc #{don.MaDonCoc} đã được xác nhận. Vui lòng đến showroom để hoàn tất thủ tục.", "/Orders/DepositResult?maDonCoc=" + don.MaDonCoc);
        await _notif.SendToRoleAsync("Admin", "Đơn cọc đã được Quản Lý tiếp nhận",
            $"Đơn cọc #{don.MaDonCoc} - {don.HoTen} - Vui lòng vào bán xe",
            $"/Admin/DonCoc/Index");

        await _log.LogAsync($"QL tiếp nhận đơn cọc #{maDonCoc}");
        SuccessMessage = "Đã tiếp nhận đơn cọc.";
        return RedirectToPage();
    }
}
