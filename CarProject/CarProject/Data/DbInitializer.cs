using CarProject.Models;

namespace CarProject.Data;

public static class DbInitializer
{
    public static void SeedData(AppDbContext context)
    {
        // Nếu đã có dữ liệu, không seed lại
        if (context.HangXe.Any())
            return;

        // Seed Hãng Xe
        var hangXeList = new List<HangXe>
        {
            new HangXe { MaHang = 1, TenHang = "Toyota", QuocGia = "Nhật Bản", DuongDanLogo = "/images/brands/toyota.png" },
            new HangXe { MaHang = 2, TenHang = "Honda", QuocGia = "Nhật Bản", DuongDanLogo = "/images/brands/honda.png" },
            new HangXe { MaHang = 3, TenHang = "Ford", QuocGia = "Mỹ", DuongDanLogo = "/images/brands/ford.png" },
            new HangXe { MaHang = 4, TenHang = "BMW", QuocGia = "Đức", DuongDanLogo = "/images/brands/bmw.png" },
            new HangXe { MaHang = 5, TenHang = "Vinfast", QuocGia = "Việt Nam", DuongDanLogo = "/images/brands/vinfast.png" }
        };
        context.HangXe.AddRange(hangXeList);
        context.SaveChanges();

        // Seed Dòng Xe
        var dongXeList = new List<DongXe>
        {
            new DongXe { MaDong = 1, MaHang = 1, TenDong = "Toyota Camry", KieuDang = "Sedan" },
            new DongXe { MaDong = 2, MaHang = 1, TenDong = "Toyota Hilux", KieuDang = "Bán tải" },
            new DongXe { MaDong = 3, MaHang = 2, TenDong = "Honda Civic", KieuDang = "Sedan" },
            new DongXe { MaDong = 4, MaHang = 2, TenDong = "Honda Pilot", KieuDang = "SUV" },
            new DongXe { MaDong = 5, MaHang = 3, TenDong = "Ford Explorer", KieuDang = "SUV" },
            new DongXe { MaDong = 6, MaHang = 4, TenDong = "BMW 3 Series", KieuDang = "Sedan" },
            new DongXe { MaDong = 7, MaHang = 5, TenDong = "VinFast Lux SA", KieuDang = "SUV" },
            new DongXe { MaDong = 8, MaHang = 5, TenDong = "VinFast Fadil", KieuDang = "Hatchback" }
        };
        context.DongXe.AddRange(dongXeList);
        context.SaveChanges();

        // Seed Phiên bản Xe (PhienBanXe_SanPham)
        var phienBanXeList = new List<PhienBanXe>
        {
            // Toyota Camry
            new PhienBanXe
            {
                MaPhienBan = 1,
                MaDong = 1,
                TenPhienBan = "Camry 2.0 CVT (Tự động)",
                GiaNiemYet = 1_050_000_000,
                MauSac = "Bạc",
                DongCo = "2.0L 4 xi-lanh",
                HopSo = "CVT",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 5,
                DuongDanAnh = "/images/cars/camry.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            new PhienBanXe
            {
                MaPhienBan = 2,
                MaDong = 1,
                TenPhienBan = "Camry 2.5 Hybrid",
                GiaNiemYet = 1_150_000_000,
                MauSac = "Đen",
                DongCo = "2.5L Hybrid",
                HopSo = "CVT",
                LoaiNhietLieu = "Xăng + Điện",
                SoLuongTrongKho = 3,
                DuongDanAnh = "/images/cars/camry-hybrid.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            // Toyota Hilux
            new PhienBanXe
            {
                MaPhienBan = 3,
                MaDong = 2,
                TenPhienBan = "Hilux 2.4L Turbo",
                GiaNiemYet = 950_000_000,
                MauSac = "Trắng",
                DongCo = "2.4L Turbo",
                HopSo = "Số tự động 6 cấp",
                LoaiNhietLieu = "Dầu",
                SoLuongTrongKho = 8,
                DuongDanAnh = "/images/cars/hilux.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            // Honda Civic
            new PhienBanXe
            {
                MaPhienBan = 4,
                MaDong = 3,
                TenPhienBan = "Civic 1.5 Turbo",
                GiaNiemYet = 750_000_000,
                MauSac = "Đỏ",
                DongCo = "1.5L Turbo",
                HopSo = "CVT",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 6,
                DuongDanAnh = "/images/cars/civic.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            // Honda Pilot
            new PhienBanXe
            {
                MaPhienBan = 5,
                MaDong = 4,
                TenPhienBan = "Pilot 3.5L AWD",
                GiaNiemYet = 1_300_000_000,
                MauSac = "Xám",
                DongCo = "3.5L V6",
                HopSo = "Số tự động 10 cấp",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 2,
                DuongDanAnh = "/images/cars/pilot.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            // Ford Explorer
            new PhienBanXe
            {
                MaPhienBan = 6,
                MaDong = 5,
                TenPhienBan = "Explorer 2.3L Eco Boost",
                GiaNiemYet = 1_200_000_000,
                MauSac = "Xanh",
                DongCo = "2.3L EcoBoost",
                HopSo = "Số tự động 10 cấp",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 4,
                DuongDanAnh = "/images/cars/explorer.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            // BMW 3 Series
            new PhienBanXe
            {
                MaPhienBan = 7,
                MaDong = 6,
                TenPhienBan = "320i Luxury",
                GiaNiemYet = 1_600_000_000,
                MauSac = "Đen",
                DongCo = "2.0L Turbo 4 xi-lanh",
                HopSo = "Số tự động 8 cấp",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 1,
                DuongDanAnh = "/images/cars/bmw320.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            // VinFast Lux SA
            new PhienBanXe
            {
                MaPhienBan = 8,
                MaDong = 7,
                TenPhienBan = "Lux SA 2.0T",
                GiaNiemYet = 1_100_000_000,
                MauSac = "Trắng",
                DongCo = "2.0L Turbo",
                HopSo = "Số tự động 8 cấp",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 7,
                DuongDanAnh = "/images/cars/luxsa.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            // VinFast Fadil
            new PhienBanXe
            {
                MaPhienBan = 9,
                MaDong = 8,
                TenPhienBan = "Fadil 1.2 MT",
                GiaNiemYet = 360_000_000,
                MauSac = "Đỏ",
                DongCo = "1.2L 3 xi-lanh",
                HopSo = "Số tay 5 cấp",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 15,
                DuongDanAnh = "/images/cars/fadil.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            },
            new PhienBanXe
            {
                MaPhienBan = 10,
                MaDong = 8,
                TenPhienBan = "Fadil 1.2 AT",
                GiaNiemYet = 395_000_000,
                MauSac = "Bạc",
                DongCo = "1.2L 3 xi-lanh",
                HopSo = "Số tự động CVT",
                LoaiNhietLieu = "Xăng",
                SoLuongTrongKho = 12,
                DuongDanAnh = "/images/cars/fadil-at.jpg",
                MaKhuyenMai = null,
                TrangThai = "Còn hàng"
            }
        };
        context.PhienBanXe.AddRange(phienBanXeList);
        context.SaveChanges();

        // Seed Quảng cáo Banner
        var bannerList = new List<QuangCaoBanner>
        {
            new QuangCaoBanner
            {
                MaBanner = 1,
                DuongDanAnh = "/images/banners/banner1.jpg",
                DuongDanLienKet = "/Details/1",
                ThuTuHienThi = 1,
                MaQuanLyCapNhat = 1,
                TrangThaiKichHoat = true
            },
            new QuangCaoBanner
            {
                MaBanner = 2,
                DuongDanAnh = "/images/banners/banner2.jpg",
                DuongDanLienKet = "/Details/3",
                ThuTuHienThi = 2,
                MaQuanLyCapNhat = 1,
                TrangThaiKichHoat = true
            },
            new QuangCaoBanner
            {
                MaBanner = 3,
                DuongDanAnh = "/images/banners/banner3.jpg",
                DuongDanLienKet = "/Details/7",
                ThuTuHienThi = 3,
                MaQuanLyCapNhat = 1,
                TrangThaiKichHoat = true
            }
        };
        context.QuangCaoBanner.AddRange(bannerList);
        context.SaveChanges();

        // Seed tài khoản quản lý mẫu
        var taiKhoanList = new List<TaiKhoan>
        {
            new TaiKhoan
            {
                MaTaiKhoan = 1,
                TenDangNhap = "admin",
                MatKhau = "admin123",
                VaiTro = "Admin",
                TrangThai = "Active"
            },
            new TaiKhoan
            {
                MaTaiKhoan = 2,
                TenDangNhap = "quanly1",
                MatKhau = "pass123",
                VaiTro = "QuanLy",
                TrangThai = "Active"
            }
        };
        context.TaiKhoan.AddRange(taiKhoanList);
        context.SaveChanges();

        // Seed Kênh Tư vấn
        var kenhTuVanList = new List<KenhTuVan>
        {
            new KenhTuVan
            {
                MaKenh = 1,
                UrlMessenger = "https://m.me/carshop",
                UrlZalo = "https://zalo.me/carshop",
                UrlSMS = "0906123456"
            }
        };
        context.KenhTuVan.AddRange(kenhTuVanList);
        context.SaveChanges();
    }
}
