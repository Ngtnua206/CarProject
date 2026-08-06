using CarProject.Data;
using CarProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CarProject.Services;

public interface ISaleService
{
    Task<(HoaDonMuaXe HoaDon, string TenXe)> SellAsync(DonDatCoc don, string nguoiXuat);
}

public class SaleService : ISaleService
{
    private readonly AppDbContext _db;

    public SaleService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(HoaDonMuaXe, string)> SellAsync(DonDatCoc don, string nguoiXuat)
    {
        var listPhienBan = don.ChiTiets.Any()
            ? don.ChiTiets.Select(c => c.PhienBan).Where(p => p != null).ToList()
            : new List<PhienBanXe?> { don.PhienBan };

        var tenXe = string.Join(", ", listPhienBan.Select(p => $"{p!.DongXe?.TenDong ?? ""} {p.TenPhienBan}".Trim()));

        var existing = await _db.HoaDonMuaXe.FirstOrDefaultAsync(h => h.MaDonCoc == don.MaDonCoc);
        if (existing != null)
        {
            don.TrangThaiDonHang = "Hoàn tất";
            await _db.SaveChangesAsync();
            return (existing, tenXe);
        }

        var tongGia = (long)listPhienBan.Sum(p => p!.GiaNiemYet);
        var hoaDon = new HoaDonMuaXe
        {
            MaHoaDon = $"HD{DateTime.Now:yyMMddHHmmss}{don.MaDonCoc:D4}",
            MaDonCoc = don.MaDonCoc,
            MaKhachHang = don.MaKhachHang,
            MaPhienBan = don.MaPhienBan ?? listPhienBan.First()!.MaPhienBan,
            MaQuanLyXuat = nguoiXuat,
            MaChiNhanh = don.MaChiNhanh,
            GiaXeThucTe = tongGia,
            ThueTruocBaVaPhiLanBanh = 0,
            SoTienDuocGiam = 0,
            TongTienPhaiTra = tongGia,
            SoTienDaThanhToan = (long)don.SoTienCoc,
            PhuongThucThanhToan = don.PhuongThucThanhToan,
            NgayXuatHoaDon = DateTime.Now,
            SoKhung = "Chưa cập nhật",
            SoMay = "Chưa cập nhật",
            TrangThaiHoaDon = "Đã thanh toán"
        };
        _db.HoaDonMuaXe.Add(hoaDon);

        foreach (var p in listPhienBan)
        {
            p!.SoLuongTrongKho--;
        }

        var kyBaoCao = DateTime.Now.ToString("yyyy-MM");
        var maChiNhanh = don.MaChiNhanh ?? "";
        var thongKe = await _db.ThongKeTongHop_Boss
            .FirstOrDefaultAsync(t => t.KyBaoCao == kyBaoCao && t.MaChiNhanh == maChiNhanh);
        if (thongKe == null)
        {
            var maDong = listPhienBan.FirstOrDefault()?.MaDong ?? 1;
            thongKe = new ThongKeTongHop_Boss
            {
                KyBaoCao = kyBaoCao,
                MaChiNhanh = maChiNhanh,
                TongDoanhThu = tongGia,
                TongTienCocThuVe = 0,
                TongSoXeDaBan = 1,
                SoDonCocBiHuy = 0,
                TongLuotXemWeb = 0,
                TongLuotLaiThu = 0,
                MaDongXeBanChayNhat = maDong
            };
            _db.ThongKeTongHop_Boss.Add(thongKe);
        }
        else
        {
            thongKe.TongDoanhThu += tongGia;
            thongKe.TongSoXeDaBan++;
        }

        don.TrangThaiDonHang = "Hoàn tất";
        await _db.SaveChangesAsync();

        return (hoaDon, tenXe);
    }
}