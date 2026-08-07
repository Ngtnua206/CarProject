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
    private readonly ISaleService _sale;
    private readonly IRevenueService _revenue;

    public DonCocModel(AppDbContext db, IActivityLogService log, INotificationService notif, ISaleService sale, IRevenueService revenue)
    {
        _db = db;
        _log = log;
        _notif = notif;
        _sale = sale;
        _revenue = revenue;
    }

    public ChiNhanhShowroom? Showroom { get; set; }
    public List<DonDatCoc> DanhSachDonCoc { get; set; } = new();
    public List<HoaDonMuaXe> HoaDonList { get; set; } = new();
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

        var maDons = DanhSachDonCoc.Select(d => d.MaDonCoc).ToList();
        HoaDonList = await _db.HoaDonMuaXe
            .Where(h => maDons.Contains(h.MaDonCoc))
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
    public async Task<IActionResult> OnPostAcceptOrderAsync(int maDonCoc, string? ngayHenNhanXe = null)
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

        // QL chốt giờ hẹn gửi lại khách khi tiếp nhận
        if (string.IsNullOrWhiteSpace(ngayHenNhanXe) || !DateTime.TryParse(ngayHenNhanXe, out var ngayHenChot))
        {
            ErrorMessage = "Vui lòng chốt ngày/giờ hẹn gặp trước khi tiếp nhận đơn cọc.";
            return RedirectToPage();
        }
        don.NgayHenNhanXe = ngayHenChot;
        don.TrangThaiDonHang = "Đã xác nhận";
        don.MaQuanLyDuyet = userName;
        await _db.SaveChangesAsync();

        var lichHenText = ngayHenChot.ToString("dd/MM/yyyy HH:mm 'giờ'");
        var diaDiemText = string.IsNullOrWhiteSpace(don.DiaDiemGap) ? showroom.TenChiNhanh + " (" + showroom.DiaChi + ")" : don.DiaDiemGap;
        await _notif.SendAsync(don.MaKhachHang!, "Đơn cọc đã được tiếp nhận - Lịch hẹn của bạn",
            $"Đơn cọc #{don.MaDonCoc} đã được xác nhận. Lịch hẹn gặp: {lichHenText} tại: {diaDiemText}. Vui lòng đến đúng giờ để hoàn tất thủ tục.",
            "/Orders/DepositResult?maDonCoc=" + don.MaDonCoc);
        await _notif.SendToRoleAsync("Admin", "Đơn cọc đã được Quản Lý tiếp nhận",
            $"Đơn cọc #{don.MaDonCoc} - {don.HoTen} - Vui lòng vào bán xe",
            $"/Admin/DonCoc/Index");

        await _log.LogAsync($"QL tiếp nhận đơn cọc #{maDonCoc} - hẹn {lichHenText}");
        SuccessMessage = "Đã tiếp nhận đơn cọc và gửi lịch hẹn cho khách.";
        return RedirectToPage();
    }

    // ----- Bán thành công: tạo hoá đơn (cơ sở + tên khách + tiền), trừ kho, cập nhật doanh thu -----
    public async Task<IActionResult> OnPostSoldAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc
            .Include(d => d.PhienBan).ThenInclude(p => p.DongXe)
            .Include(d => d.ChiTiets).ThenInclude(c => c.PhienBan).ThenInclude(p => p.DongXe)
            .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
        if (don == null) return NotFound();
        if (!don.ChiTiets.Any() && don.PhienBan == null) return NotFound();

        var showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (showroom == null || don.MaChiNhanh != showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền bán đơn cọc của showroom khác.";
            return RedirectToPage();
        }

        if (don.TrangThaiDonHang != "Đã xác nhận")
        {
            ErrorMessage = "Chỉ đơn cọc đã tiếp nhận (Đã xác nhận) mới được bán.";
            return RedirectToPage();
        }

        var (hoaDon, tenXe) = await _sale.SellAsync(don, userName);

        await _notif.SendAsync(don.MaKhachHang!, "Mua xe thành công",
            $"Xe {tenXe} - Đơn cọc #{maDonCoc} đã hoàn tất. Hóa đơn {hoaDon.MaHoaDon} được xuất tại {showroom.TenChiNhanh}. Cảm ơn bạn đã mua xe!",
            $"/Orders/DepositResult?maDonCoc={maDonCoc}");
        await _notif.SendToRoleAsync("Admin", "Quản Lý đã bán xe thành công",
            $"Đơn cọc #{maDonCoc} - {don.HoTen} - xe {tenXe} đã bán tại {showroom.TenChiNhanh}. Hóa đơn {hoaDon.MaHoaDon}.",
            "/Admin/HoaDon");

        await _log.LogAsync($"QL bán thành công đơn cọc #{maDonCoc} - {tenXe} (HĐ {hoaDon.MaHoaDon})");
        SuccessMessage = $"Đã bán thành công xe {tenXe}. Hóa đơn {hoaDon.MaHoaDon} đã được tạo.";
        return RedirectToPage();
    }

    // ----- Không bán / Huỷ giao dịch (Quản Lý) -----
    public async Task<IActionResult> OnPostCancelSaleAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc
            .Include(d => d.PhienBan)
            .Include(d => d.ChiTiets).ThenInclude(c => c.PhienBan)
            .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
        if (don == null) return NotFound();

        var showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (showroom == null || don.MaChiNhanh != showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền hủy giao dịch của showroom khác.";
            return RedirectToPage();
        }

        if (don.TrangThaiDonHang != "Đã xác nhận" && don.TrangThaiDonHang != "Hoàn tất")
        {
            ErrorMessage = "Chỉ đơn đã tiếp nhận (Đã xác nhận) mới có thể hủy bán.";
            return RedirectToPage();
        }

        // Nếu đơn đã bán (Hoàn tất): xoá hoá đơn + hoàn doanh thu + trả lại xe
        var hoaDon = await _db.HoaDonMuaXe.FirstOrDefaultAsync(h => h.MaDonCoc == maDonCoc);
        if (hoaDon != null)
        {
            await HoanTacDoanhThu(don.MaChiNhanh ?? "", hoaDon.TongTienPhaiTra);
            _db.HoaDonMuaXe.Remove(hoaDon);

            var listPhienBan = don.ChiTiets.Any()
                ? don.ChiTiets.Select(c => c.PhienBan).Where(p => p != null).ToList()
                : new List<PhienBanXe?> { don.PhienBan };
            foreach (var p in listPhienBan)
            {
                if (p != null) p.SoLuongTrongKho++;
            }
        }

        don.TrangThaiDonHang = "Đã hủy";
        await _revenue.RevertDepositRevenueAsync(maDonCoc);
        await _db.SaveChangesAsync();

        await _notif.SendAsync(don.MaKhachHang!, "Giao dịch bị hủy",
            $"Giao dịch đơn cọc #{maDonCoc} tại {showroom.TenChiNhanh} đã bị hủy bán. Tiền cọc sẽ được hoàn lại.", "");
        await _notif.SendToRoleAsync("Admin", "Quản Lý hủy bán giao dịch",
            $"Đơn cọc #{maDonCoc} - {don.HoTen} đã bị Quản Lý hủy bán tại {showroom.TenChiNhanh}.", "/Admin/DonCoc");

        await _log.LogAsync($"QL hủy bán (Không bán) đơn cọc #{maDonCoc}");
        SuccessMessage = "Đã hủy giao dịch và trả xe về kho.";
        return RedirectToPage();
    }

    private async Task HoanTacDoanhThu(string maChiNhanh, long soTien)
    {
        var kyBaoCao = DateTime.Now.ToString("yyyy-MM");
        var thongKe = await _db.ThongKeTongHop_Boss
            .FirstOrDefaultAsync(t => t.KyBaoCao == kyBaoCao && t.MaChiNhanh == maChiNhanh);
        if (thongKe != null)
        {
            thongKe.TongDoanhThu -= soTien;
            thongKe.TongSoXeDaBan--;
            if (thongKe.TongSoXeDaBan < 0) thongKe.TongSoXeDaBan = 0;
            if (thongKe.TongDoanhThu < 0) thongKe.TongDoanhThu = 0;
        }
    }
}
