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

        // Nếu tất cả xe đã tiếp nhận thì đơn chuyển sang Đã xác nhận
        var allAccepted = await _db.DonDatCocChiTiet
            .AllAsync(c => c.MaDonCoc == chiTiet.MaDonCoc
                && (c.TrangThaiTiepNhan == "Đã tiếp nhận" || c.TrangThaiTiepNhan == "Từ chối"));
        if (allAccepted && chiTiet.DonDatCoc != null)
        {
            chiTiet.DonDatCoc.TrangThaiDonHang = "Đã xác nhận";
            chiTiet.DonDatCoc.MaQuanLyDuyet = userName;
            await _db.SaveChangesAsync();

            var don = chiTiet.DonDatCoc;
            await _notif.SendAsync(don.MaKhachHang!, "Đơn cọc đã được xác nhận",
                $"Đơn cọc #{don.MaDonCoc} đã được chấp nhận. Vui lòng đến showroom để hoàn tất thủ tục.", "/Orders/DepositResult?maDonCoc=" + don.MaDonCoc);
            await _notif.SendToRoleAsync("Admin", "Đơn cọc đã được Quản Lý xác nhận",
                $"Đơn cọc #{don.MaDonCoc} - {don.HoTen} - Vui lòng vào bán xe",
                $"/Admin/DonCoc/Index");
        }

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
}
