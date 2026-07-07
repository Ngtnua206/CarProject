-- Tài khoản
SET IDENTITY_INSERT CarShopDb.dbo.TaiKhoan ON;
IF NOT EXISTS (SELECT 1 FROM CarShopDb.dbo.TaiKhoan) BEGIN
INSERT CarShopDb.dbo.TaiKhoan (MaTaiKhoan, TenDangNhap, MatKhau, VaiTro, TrangThai) VALUES
(1, 'admin', 'admin123', 'Admin', 'Active'),
(2, 'quanly1', 'pass123', N'Quản Lý', 'Active')
END
SET IDENTITY_INSERT CarShopDb.dbo.TaiKhoan OFF;

-- Xóa dữ liệu cũ
DELETE FROM CarShopDb.dbo.QuangCaoBanner;
DELETE FROM CarShopDb.dbo.PhienBanXe_SanPham;
DELETE FROM CarShopDb.dbo.DongXe;
DELETE FROM CarShopDb.dbo.HangXe;

-- Hãng Xe
SET IDENTITY_INSERT CarShopDb.dbo.HangXe ON;
INSERT CarShopDb.dbo.HangXe (MaHang, TenHang, QuocGia, DuongDanLogo) VALUES
(1, N'Toyota', N'Nhật Bản', '/images/brands/toyota.png'),
(2, N'Honda', N'Nhật Bản', '/images/brands/honda.png'),
(3, N'Ford', N'Mỹ', '/images/brands/ford.png'),
(4, N'BMW', N'Đức', '/images/brands/bmw.png'),
(5, N'Vinfast', N'Việt Nam', '/images/brands/vinfast.png')
SET IDENTITY_INSERT CarShopDb.dbo.HangXe OFF;

-- Dòng Xe
SET IDENTITY_INSERT CarShopDb.dbo.DongXe ON;
INSERT CarShopDb.dbo.DongXe (MaDong, MaHang, TenDong, KieuDang) VALUES
(1, 1, N'Toyota Camry', N'Sedan'),
(2, 1, N'Toyota Hilux', N'Bán tải'),
(3, 2, N'Honda Civic', N'Sedan'),
(4, 2, N'Honda Pilot', N'SUV'),
(5, 3, N'Ford Explorer', N'SUV'),
(6, 4, N'BMW 3 Series', N'Sedan'),
(7, 5, N'VinFast Lux SA', N'SUV'),
(8, 5, N'VinFast Fadil', N'Hatchback')
SET IDENTITY_INSERT CarShopDb.dbo.DongXe OFF;

-- Phiên bản
SET IDENTITY_INSERT CarShopDb.dbo.PhienBanXe_SanPham ON;
INSERT CarShopDb.dbo.PhienBanXe_SanPham (MaPhienBan, MaDong, TenPhienBan, GiaNiemYet, MauSac, DongCo, HopSo, LoaiNhietLieu, SoLuongTrongKho, DuongDanAnh, MaKhuyenMai, TrangThai) VALUES
(1, 1, N'Camry 2.0 CVT (Tự động)', 1050000000, N'Bạc', N'2.0L 4 xi-lanh', N'CVT', N'Xăng', 5, '/images/cars/camry.jpg', '', N'Còn hàng'),
(2, 1, N'Camry 2.5 Hybrid', 1150000000, N'Đen', N'2.5L Hybrid', N'CVT', N'Xăng + Điện', 3, '/images/cars/camry-hybrid.jpg', '', N'Còn hàng'),
(3, 2, N'Hilux 2.4L Turbo', 950000000, N'Trắng', N'2.4L Turbo', N'Số tự động 6 cấp', N'Dầu', 8, '/images/cars/hilux.jpg', '', N'Còn hàng'),
(4, 3, N'Civic 1.5 Turbo', 750000000, N'Đỏ', N'1.5L Turbo', N'CVT', N'Xăng', 6, '/images/cars/civic.jpg', '', N'Còn hàng'),
(5, 4, N'Pilot 3.5L AWD', 1300000000, N'Xám', N'3.5L V6', N'Số tự động 10 cấp', N'Xăng', 2, '/images/cars/pilot.jpg', '', N'Còn hàng'),
(6, 5, N'Explorer 2.3L Eco Boost', 1200000000, N'Xanh', N'2.3L EcoBoost', N'Số tự động 10 cấp', N'Xăng', 4, '/images/cars/explorer.jpg', '', N'Còn hàng'),
(7, 6, N'320i Luxury', 1600000000, N'Đen', N'2.0L Turbo 4 xi-lanh', N'Số tự động 8 cấp', N'Xăng', 1, '/images/cars/bmw320.jpg', '', N'Còn hàng'),
(8, 7, N'Lux SA 2.0T', 1100000000, N'Trắng', N'2.0L Turbo', N'Số tự động 8 cấp', N'Xăng', 7, '/images/cars/luxsa.jpg', '', N'Còn hàng'),
(9, 8, N'Fadil 1.2 MT', 360000000, N'Đỏ', N'1.2L 3 xi-lanh', N'Số tay 5 cấp', N'Xăng', 15, '/images/cars/fadil.jpg', '', N'Còn hàng'),
(10, 8, N'Fadil 1.2 AT', 395000000, N'Bạc', N'1.2L 3 xi-lanh', N'Số tự động CVT', N'Xăng', 12, '/images/cars/fadil-at.jpg', '', N'Còn hàng')
SET IDENTITY_INSERT CarShopDb.dbo.PhienBanXe_SanPham OFF;

-- Banner
SET IDENTITY_INSERT CarShopDb.dbo.QuangCaoBanner ON;
INSERT CarShopDb.dbo.QuangCaoBanner (MaBanner, DuongDanAnh, DuongDanLienKet, ThuTuHienThi, MaQuanLyCapNhat, TrangThaiKichHoat) VALUES
(1, '/images/banners/banner1.jpg', '/Details/1', 1, 1, 1),
(2, '/images/banners/banner2.jpg', '/Details/3', 2, 1, 1),
(3, '/images/banners/banner3.jpg', '/Details/7', 3, 1, 1)
SET IDENTITY_INSERT CarShopDb.dbo.QuangCaoBanner OFF;

-- Kênh tư vấn
SET IDENTITY_INSERT CarShopDb.dbo.KenhTuVan ON;
IF NOT EXISTS (SELECT 1 FROM CarShopDb.dbo.KenhTuVan) BEGIN
INSERT CarShopDb.dbo.KenhTuVan (MaKenh, UrlMessenger, UrlZalo, UrlSMS) VALUES
(1, 'https://m.me/carshop', 'https://zalo.me/carshop', '0906123456')
END
SET IDENTITY_INSERT CarShopDb.dbo.KenhTuVan OFF;