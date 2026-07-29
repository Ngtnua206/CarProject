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

    // ----- Bán thành công -----
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
        await _notif.SendToRoleAsync("Admin", "Xe đã bán thành công",
            $"QL {userName} đã bán {tenXe} - Mã đơn #{maDonCoc}",
            $"/Admin/DonCoc/Edit?maDonCoc={maDonCoc}");
        await _log.LogAsync($"QL bán thành công đơn cọc #{maDonCoc} - {tenXe}");

        SuccessMessage = $"Đã bán thành công xe {tenXe}.";
        return RedirectToPage();
    }

    // ----- Huỷ bán (sau khi đã chấp nhận) -----
    public async Task<IActionResult> OnPostCancelSaleAsync(int maDonCoc)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        var don = await _db.DonDatCoc
            .Include(d => d.PhienBan)
            .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
        if (don == null) return NotFound();

        // Kiểm tra nếu đã có hoá đơn thì phải trừ lại doanh thu
        var hoaDon = await _db.HoaDonMuaXe.FirstOrDefaultAsync(h => h.MaDonCoc == maDonCoc);
        if (hoaDon != null)
        {
            // Đã bán thành công trước đó → hoàn tác doanh thu
            await HoanTacDoanhThu(don.MaChiNhanh ?? "", hoaDon.TongTienPhaiTra);
            _db.HoaDonMuaXe.Remove(hoaDon);

            // Phục hồi tồn kho
            if (don.PhienBan != null)
                don.PhienBan.SoLuongTrongKho++;
        }

        don.TrangThaiDonHang = "Đã hủy";
        await _db.SaveChangesAsync();

        await _notif.SendAsync(don.MaKhachHang!, "Giao dịch bị hủy",
            $"Giao dịch đơn cọc #{maDonCoc} đã bị hủy. Tiền cọc sẽ được hoàn lại.", "");
        await _log.LogAsync($"QL hủy bán đơn cọc #{maDonCoc}");

        SuccessMessage = "Đã hủy giao dịch. Tiền cọc sẽ được hoàn lại cho khách.";
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
