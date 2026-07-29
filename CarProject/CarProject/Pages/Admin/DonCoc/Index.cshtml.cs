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
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

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

    // ----- Bán thành công (Admin chuyển xe) -----
    public async Task<IActionResult> OnPostSoldAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc
            .Include(d => d.PhienBan)
            .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
        if (don == null || don.PhienBan == null) return NotFound();

        // 1. Tạo hoá đơn
        var hoaDon = new HoaDonMuaXe
        {
            MaHoaDon = $"HD{DateTime.Now:yyMMddHHmmss}{maDonCoc:D4}",
            MaDonCoc = maDonCoc,
            MaKhachHang = don.MaKhachHang,
            MaPhienBan = don.MaPhienBan,
            MaQuanLyXuat = userName,
            MaChiNhanh = don.MaChiNhanh,
            GiaXeThucTe = don.PhienBan.GiaNiemYet,
            ThueTruocBaVaPhiLanBanh = 0,
            SoTienDuocGiam = 0,
            TongTienPhaiTra = don.PhienBan.GiaNiemYet,
            SoTienDaThanhToan = (long)don.SoTienCoc,
            PhuongThucThanhToan = don.PhuongThucThanhToan,
            NgayXuatHoaDon = DateTime.Now,
            SoKhung = "Chưa cập nhật",
            SoMay = "Chưa cập nhật",
            TrangThaiHoaDon = "Đã thanh toán"
        };
        _db.HoaDonMuaXe.Add(hoaDon);

        // 2. Trừ tồn kho
        don.PhienBan.SoLuongTrongKho--;

        // 3. Cập nhật thống kê doanh thu
        await CapNhatDoanhThu(don.MaChiNhanh ?? "", don.PhienBan.GiaNiemYet, don.MaPhienBan);

        // 4. Cập nhật trạng thái đơn
        don.TrangThaiDonHang = "Hoàn tất";
        await _db.SaveChangesAsync();

        var tenXe = $"{don.PhienBan.DongXe?.TenDong ?? ""} {don.PhienBan.TenPhienBan}".Trim();
        await _notif.SendAsync(don.MaKhachHang!, "Mua xe thành công",
            $"Xe {tenXe} - Đơn cọc #{maDonCoc} đã hoàn tất. Cảm ơn bạn đã mua xe!", "");

        var qlOfShowroom = await _db.ChiNhanhShowroom
            .Where(c => c.MaChiNhanh == don.MaChiNhanh && c.MaQuanLy != null)
            .Select(c => c.MaQuanLy)
            .FirstOrDefaultAsync();
        if (qlOfShowroom != null)
        {
            await _notif.SendAsync(qlOfShowroom, "Xe đã bán thành công",
                $"Xe {tenXe} - Đơn cọc #{maDonCoc} đã bán tại showroom của bạn",
                $"/QuanLy/DonCoc");
        }

        await _log.LogAsync($"Admin bán thành công đơn cọc #{maDonCoc} - {tenXe}");
        TempData["Success"] = $"Đã bán thành công xe {tenXe}.";
        return RedirectToPage();
    }

    // ----- Huỷ bán (Admin) -----
    public async Task<IActionResult> OnPostCancelSaleAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc
            .Include(d => d.PhienBan)
            .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
        if (don == null) return NotFound();

        var hoaDon = await _db.HoaDonMuaXe.FirstOrDefaultAsync(h => h.MaDonCoc == maDonCoc);
        if (hoaDon != null)
        {
            await HoanTacDoanhThu(don.MaChiNhanh ?? "", hoaDon.TongTienPhaiTra);
            _db.HoaDonMuaXe.Remove(hoaDon);

            if (don.PhienBan != null)
                don.PhienBan.SoLuongTrongKho++;
        }

        don.TrangThaiDonHang = "Đã hủy";
        await _db.SaveChangesAsync();

        await _notif.SendAsync(don.MaKhachHang!, "Giao dịch bị hủy",
            $"Giao dịch đơn cọc #{maDonCoc} đã bị hủy. Tiền cọc sẽ được hoàn lại.", "");

        var qlOfShowroom = await _db.ChiNhanhShowroom
            .Where(c => c.MaChiNhanh == don.MaChiNhanh && c.MaQuanLy != null)
            .Select(c => c.MaQuanLy)
            .FirstOrDefaultAsync();
        if (qlOfShowroom != null)
        {
            await _notif.SendAsync(qlOfShowroom, "Giao dịch bị hủy",
                $"Đơn cọc #{maDonCoc} tại showroom đã bị Admin hủy",
                $"/QuanLy/DonCoc");
        }

        await _log.LogAsync($"Admin hủy bán đơn cọc #{maDonCoc}");
        TempData["Error"] = $"Đã hủy giao dịch đơn cọc #{maDonCoc}.";
        return RedirectToPage();
    }

    // === Hỗ trợ doanh thu ===
    private async Task CapNhatDoanhThu(string maChiNhanh, long soTien, int maPhienBan)
    {
        var kyBaoCao = DateTime.Now.ToString("yyyy-MM");
        var thongKe = await _db.ThongKeTongHop_Boss
            .FirstOrDefaultAsync(t => t.KyBaoCao == kyBaoCao && t.MaChiNhanh == maChiNhanh);

        if (thongKe == null)
        {
            thongKe = new ThongKeTongHop_Boss
            {
                KyBaoCao = kyBaoCao,
                MaChiNhanh = maChiNhanh,
                TongDoanhThu = soTien,
                TongTienCocThuVe = 0,
                TongSoXeDaBan = 1,
                SoDonCocBiHuy = 0,
                TongLuotXemWeb = 0,
                TongLuotLaiThu = 0,
                MaDongXeBanChayNhat = maPhienBan
            };
            _db.ThongKeTongHop_Boss.Add(thongKe);
        }
        else
        {
            thongKe.TongDoanhThu += soTien;
            thongKe.TongSoXeDaBan++;
        }
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