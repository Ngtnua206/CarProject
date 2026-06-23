using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarProject.Models;

public class DonDatCoc
{
    [Key]
    public int MaDonCoc { get; set; }
    public int MaKhachHang { get; set; }
    public int MaPhienBan { get; set; }
    public int? MaQuanLyDuyet { get; set; }
    public decimal SoTienCoc { get; set; }
    public string PhuongThucThanhToan { get; set; }
    public string TrangThaiThanhToan { get; set; }
    public DateTime NgayTaoDon { get; set; }
    public DateTime? NgayHenNhanXe { get; set; }
    public string TrangThaiDonHang { get; set; }
    public string GhiChu { get; set; }

    [ForeignKey("MaKhachHang")]
    public TaiKhoan KhachHang { get; set; }

    [ForeignKey("MaPhienBan")]
    public PhienBanXe PhienBan { get; set; }

    [ForeignKey("MaQuanLyDuyet")]
    public TaiKhoan QuanLyDuyet { get; set; }
}

public class HoaDonMuaXe
{
    [Key]
    public string MaHoaDon { get; set; }
    public int MaDonCoc { get; set; }
    public int MaKhachHang { get; set; }
    public int MaPhienBan { get; set; }
    public int MaQuanLyXuat { get; set; }
    public long GiaXeThucTe { get; set; }
    public long ThueTruocBaVaPhiLanBanh { get; set; }
    public long SoTienDuocGiam { get; set; }
    public long TongTienPhaiTra { get; set; }
    public long SoTienDaThanhToan { get; set; }
    public string PhuongThucThanhToan { get; set; }
    public DateTime NgayXuatHoaDon { get; set; }
    public string SoKhung { get; set; }
    public string SoMay { get; set; }
    public string TrangThaiHoaDon { get; set; }

    [ForeignKey("MaDonCoc")]
    public DonDatCoc DonDatCoc { get; set; }
}

public class LichHenLaiThu
{
    [Key]
    public int MaLichHen { get; set; }
    public int MaKhachHang { get; set; }
    public int MaDong { get; set; }
    public string MaChiNhanh { get; set; }
    public string HoTenNguoiLai { get; set; }
    public string SoDienThoai { get; set; }
    public string SoBangLaiXe { get; set; }
    public DateTime NgayHen { get; set; }
    public string GioHen { get; set; }
    public string TrangThai { get; set; }
    public string YKienKhachHang { get; set; }

    [ForeignKey("MaKhachHang")]
    public TaiKhoan KhachHang { get; set; }

    [ForeignKey("MaDong")]
    public DongXe DongXe { get; set; }

    [ForeignKey("MaChiNhanh")]
    public ChiNhanhShowroom ChiNhanh { get; set; }
}

public class ChiNhanhShowroom
{
    [Key]
    public string MaChiNhanh { get; set; }
    public string TenChiNhanh { get; set; }
    public string DiaChi { get; set; }
    public string ThanhPho { get; set; }
    public string DuongDayNong { get; set; }
    public int MaQuanLy { get; set; }
    public string TrangThai { get; set; }

    [ForeignKey("MaQuanLy")]
    public TaiKhoan QuanLy { get; set; }
}

public class ChuongTrinhKhuyenMai
{
    [Key]
    public string MaKhuyenMai { get; set; }
    public string TieuDe { get; set; }
    public string MoTa { get; set; }
    public string LoaiGiamGia { get; set; }
    public decimal GiaTriGiam { get; set; }
    public decimal MucGiamToiDa { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string TrangThai { get; set; }
}

public class QuangCaoBanner
{
    [Key]
    public int MaBanner { get; set; }
    public string DuongDanAnh { get; set; }
    public string DuongDanLienKet { get; set; }
    public int ThuTuHienThi { get; set; }
    public int MaQuanLyCapNhat { get; set; }
    public bool TrangThaiKichHoat { get; set; }

    [ForeignKey("MaQuanLyCapNhat")]
    public TaiKhoan QuanLyCapNhat { get; set; }
}

public class KenhTuVan
{
    [Key]
    public int MaKenh { get; set; }
    public string UrlMessenger { get; set; }
    public string UrlZalo { get; set; }
    public string UrlSMS { get; set; }
}

public class NhatKyHeThong
{
    [Key]
    public int MaNhatKy { get; set; }
    public int? MaTaiKhoan { get; set; }
    public string TenDangNhap { get; set; }
    public string VaiTro { get; set; }
    public string HanhDong { get; set; }
    public string ChiTiet { get; set; }
    public string DiaChiIP { get; set; }
    public string TrinhDuyet { get; set; }
    public string DuongDan { get; set; }
    public DateTime ThoiGian { get; set; }

    [ForeignKey("MaTaiKhoan")]
    public TaiKhoan TaiKhoan { get; set; }
}

public class ThongKeTongHop_Boss
{
    [Key]
    public int MaThongKe { get; set; }
    public string KyBaoCao { get; set; }
    public string MaChiNhanh { get; set; }
    public long TongDoanhThu { get; set; }
    public long TongTienCocThuVe { get; set; }
    public int TongSoXeDaBan { get; set; }
    public int SoDonCocBiHuy { get; set; }
    public int TongLuotXemWeb { get; set; }
    public int TongLuotLaiThu { get; set; }
    public int MaDongXeBanChayNhat { get; set; }

    [ForeignKey("MaChiNhanh")]
    public ChiNhanhShowroom ChiNhanh { get; set; }

    [ForeignKey("MaDongXeBanChayNhat")]
    public DongXe DongXeBanChay { get; set; }
}
