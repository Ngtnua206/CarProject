-- ===================================================
-- SQL thao tác nhanh cho CarProject
-- Chạy trong SSMS (chọn database CarShopDb trước)
-- Thứ tự từ trên xuống dưới, chạy phát ok luôn
-- ===================================================

-- ==================== 0. CẬP NHẬT SCHEMA ====================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DonDatCoc') AND name = 'MaChiNhanh')
    ALTER TABLE DonDatCoc ADD MaChiNhanh NVARCHAR(450) NULL;
ELSE IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DonDatCoc') AND name = 'MaChiNhanh' AND max_length = 40)
    ALTER TABLE DonDatCoc ALTER COLUMN MaChiNhanh NVARCHAR(450) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HoaDonMuaXe') AND name = 'MaChiNhanh')
    ALTER TABLE HoaDonMuaXe ADD MaChiNhanh NVARCHAR(450) NULL;
ELSE IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('HoaDonMuaXe') AND name = 'MaChiNhanh' AND max_length = 40)
    ALTER TABLE HoaDonMuaXe ALTER COLUMN MaChiNhanh NVARCHAR(450) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DongXe') AND name = 'NoiBat')
    ALTER TABLE DongXe ADD NoiBat BIT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DonDatCoc_ChiNhanhShowroom')
    ALTER TABLE DonDatCoc ADD CONSTRAINT FK_DonDatCoc_ChiNhanhShowroom FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanhShowroom(MaChiNhanh);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_HoaDonMuaXe_ChiNhanhShowroom')
    ALTER TABLE HoaDonMuaXe ADD CONSTRAINT FK_HoaDonMuaXe_ChiNhanhShowroom FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanhShowroom(MaChiNhanh);
-- Bảng chi tiết đơn cọc (hỗ trợ đặt trước xe hết hàng)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DonDatCocChiTiet')
BEGIN
    CREATE TABLE DonDatCocChiTiet (
        MaChiTiet INT IDENTITY(1,1) NOT NULL,
        MaDonCoc INT NOT NULL,
        MaPhienBan INT NOT NULL,
        MaChiNhanh NVARCHAR(450) NULL,
        SoLuong INT NOT NULL,
        TrangThaiTiepNhan NVARCHAR(MAX) NOT NULL DEFAULT N'Chờ xác nhận',
        LyDoTuChoi NVARCHAR(MAX) NULL,
        NguoiPhanHoi NVARCHAR(MAX) NULL,
        NgayPhanHoi DATETIME2 NULL,
        CONSTRAINT PK_DonDatCocChiTiet PRIMARY KEY (MaChiTiet),
        CONSTRAINT FK_DonDatCocChiTiet_ChiNhanhShowroom_MaChiNhanh FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanhShowroom(MaChiNhanh) ON DELETE NO ACTION,
        CONSTRAINT FK_DonDatCocChiTiet_DonDatCoc_MaDonCoc FOREIGN KEY (MaDonCoc) REFERENCES DonDatCoc(MaDonCoc) ON DELETE CASCADE,
        CONSTRAINT FK_DonDatCocChiTiet_PhienBanXe_SanPham_MaPhienBan FOREIGN KEY (MaPhienBan) REFERENCES PhienBanXe_SanPham(MaPhienBan) ON DELETE NO ACTION
    );
    CREATE INDEX IX_DonDatCocChiTiet_MaChiNhanh ON DonDatCocChiTiet(MaChiNhanh);
    CREATE INDEX IX_DonDatCocChiTiet_MaDonCoc ON DonDatCocChiTiet(MaDonCoc);
    CREATE INDEX IX_DonDatCocChiTiet_MaPhienBan ON DonDatCocChiTiet(MaPhienBan);
END
-- Bảng tồn kho theo chi nhánh
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TonKhoTheoChiNhanh')
BEGIN
    CREATE TABLE TonKhoTheoChiNhanh (
        MaTonKho INT IDENTITY(1,1) NOT NULL,
        MaPhienBan INT NOT NULL,
        MaChiNhanh NVARCHAR(450) NOT NULL,
        SoLuong INT NOT NULL,
        NgayCapNhat DATETIME2 NOT NULL,
        CONSTRAINT PK_TonKhoTheoChiNhanh PRIMARY KEY (MaTonKho),
        CONSTRAINT FK_TonKhoTheoChiNhanh_ChiNhanhShowroom_MaChiNhanh FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanhShowroom(MaChiNhanh) ON DELETE NO ACTION,
        CONSTRAINT FK_TonKhoTheoChiNhanh_PhienBanXe_SanPham_MaPhienBan FOREIGN KEY (MaPhienBan) REFERENCES PhienBanXe_SanPham(MaPhienBan) ON DELETE NO ACTION
    );
    CREATE INDEX IX_TonKhoTheoChiNhanh_MaChiNhanh ON TonKhoTheoChiNhanh(MaChiNhanh);
    CREATE INDEX IX_TonKhoTheoChiNhanh_MaPhienBan ON TonKhoTheoChiNhanh(MaPhienBan);
END
GO

-- ==================== 1. XÓA DỮ LIỆU CŨ ====================
DELETE FROM ThongKeTongHop_Boss;
DELETE FROM HoaDonMuaXe;
DELETE FROM DonDatCocChiTiet;
DELETE FROM DonDatCoc;
DELETE FROM TonKhoTheoChiNhanh;
DELETE FROM NhatKyHeThong;
DELETE FROM ThongBao;
DELETE FROM GioHang;
DELETE FROM QuangCaoBanner;
DELETE FROM LichHenLaiThu;
DELETE FROM KenhTuVan;
DELETE FROM ChuongTrinhKhuyenMai;
DELETE FROM ChiNhanhShowroom;
DELETE FROM PhienBanXe_SanPham;
DELETE FROM DongXe;
DELETE FROM HangXe;
DELETE FROM TaiKhoan;

-- ==================== 2. THÊM TÀI KHOẢN ====================
-- Admin
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('fntzzs682@gmail.com', '123456', 'Admin', 'Active', 'Admin', 'fntzzs682@gmail.com');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('Ngttu2006@gmail.com', 'Iumaioanhh@2024', 'Admin', 'Active', N'Ngtnua', 'Ngttu2006@gmail.com');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('thanhdac223@gmail.com', '@Trinhvu1', 'Admin', 'Active', '', 'thanhdac223@gmail.com');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('Vanh280306@gmail.com', 'Vanh2803', 'Admin', 'Active', '', 'Vanh280306@gmail.com');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('minhquanmkp123@gmail.com', 'hahahihi123', 'Admin', 'Active', '', 'minhquanmkp123@gmail.com');

-- User
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('user1', 'user123', 'User', 'Active', N'Nguyễn Văn User');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('user2', 'user123', 'User', 'Active', N'Trần Thị Khách');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('user3', 'user123', 'User', 'Active', N'Lê Hoàng Nam');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('user4', 'user123', 'User', 'Active', N'Phạm Minh Tâm');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('user5', 'user123', 'User', 'Active', N'Đỗ Thúy Hằng');

-- Quản Lý
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('QuanlyCS1', 'quanly123', N'Quản Lý', N'Hoạt động', N'Nguyễn Văn A');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('QuanlyCS2', 'quanly123', N'Quản Lý', N'Hoạt động', N'Nguyễn Văn B');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('QuanlyCS3', 'quanly123', N'Quản Lý', N'Hoạt động', N'Trần Thị C');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('QuanlyCS4', 'quanly123', N'Quản Lý', N'Hoạt động', N'Lê Văn D');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('QuanlyCS5', 'quanly123', N'Quản Lý', N'Hoạt động', N'Phạm Thị E');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('QuanlyCS6', 'quanly123', N'Quản Lý', N'Hoạt động', N'Hoàng Văn F');

-- ==================== 3. THÊM HÃNG XE ====================
SET IDENTITY_INSERT HangXe ON;
INSERT INTO HangXe (MaHang, TenHang, QuocGia, DuongDanLogo) VALUES
(1,  N'Toyota',        N'Nhật Bản',   '/images/brands/toyota.png'),
(2,  N'Honda',         N'Nhật Bản',   '/images/brands/honda.png'),
(3,  N'Ford',          N'Mỹ',         '/images/brands/ford.png'),
(4,  N'BMW',           N'Đức',        '/images/brands/bmw.png'),
(5,  N'VinFast',       N'Việt Nam',   '/images/brands/vinfast.png'),
(6,  N'Mercedes-Benz', N'Đức',        '/images/brands/mercedes.png'),
(7,  N'Audi',          N'Đức',        '/images/brands/audi.png'),
(8,  N'Lexus',         N'Nhật Bản',   '/images/brands/lexus.png'),
(9,  N'Hyundai',       N'Hàn Quốc',   '/images/brands/hyundai.png'),
(10, N'Kia',           N'Hàn Quốc',   '/images/brands/kia.png'),
(11, N'Mazda',         N'Nhật Bản',   '/images/brands/mazda.png'),
(12, N'Suzuki',        N'Nhật Bản',   '/images/brands/suzuki.png'),
(13, N'Mitsubishi',    N'Nhật Bản',   '/images/brands/mitsubishi.png'),
(14, N'Nissan',        N'Nhật Bản',   '/images/brands/nissan.png'),
(15, N'Subaru',        N'Nhật Bản',   '/images/brands/subaru.png');
SET IDENTITY_INSERT HangXe OFF;

-- ==================== 4. THÊM DÒNG XE ====================
SET IDENTITY_INSERT DongXe ON;
INSERT INTO DongXe (MaDong, MaHang, TenDong, KieuDang, NoiBat) VALUES
-- Toyota (1)
(1,  1,  N'Toyota Camry',          N'Sedan',     1),
(2,  1,  N'Toyota Hilux',          N'Bán tải',   0),
(3,  1,  N'Toyota Corolla Altis',  N'Sedan',     0),
(4,  1,  N'Toyota Fortuner',       N'SUV',       1),
(5,  1,  N'Toyota Vios',           N'Sedan',     0),
-- Honda (2)
(6,  2,  N'Honda Civic',           N'Sedan',     1),
(7,  2,  N'Honda CR-V',            N'SUV',       1),
(8,  2,  N'Honda HR-V',            N'SUV',       0),
(9,  2,  N'Honda Accord',          N'Sedan',     0),
-- Ford (3)
(10, 3,  N'Ford Explorer',         N'SUV',       0),
(11, 3,  N'Ford Everest',          N'SUV',       1),
(12, 3,  N'Ford Ranger',           N'Bán tải',   1),
(13, 3,  N'Ford Territory',        N'SUV',       1),
-- BMW (4)
(14, 4,  N'BMW 3 Series',          N'Sedan',     1),
(15, 4,  N'BMW 5 Series',          N'Sedan',     1),
(16, 4,  N'BMW X3',                N'SUV',       1),
(17, 4,  N'BMW X5',                N'SUV',       1),
-- VinFast (5)
(18, 5,  N'VinFast Lux SA',        N'SUV',       1),
(19, 5,  N'VinFast Fadil',         N'Hatchback', 0),
(20, 5,  N'VinFast VF e34',        N'SUV (Điện)',0),
(21, 5,  N'VinFast VF 8',          N'SUV (Điện)',1),
(22, 5,  N'VinFast VF 9',          N'SUV (Điện)',1),
-- Mercedes-Benz (6)
(23, 6,  N'Mercedes C-Class',      N'Sedan',     1),
(24, 6,  N'Mercedes E-Class',      N'Sedan',     1),
(25, 6,  N'Mercedes S-Class',      N'Sedan',     1),
(26, 6,  N'Mercedes GLC',          N'SUV',       0),
(27, 6,  N'Mercedes GLE',          N'SUV',       0),
-- Audi (7)
(28, 7,  N'Audi A3',               N'Sedan',     0),
(29, 7,  N'Audi A4',               N'Sedan',     1),
(30, 7,  N'Audi Q5',               N'SUV',       1),
(31, 7,  N'Audi Q7',               N'SUV',       0),
-- Lexus (8)
(32, 8,  N'Lexus ES',              N'Sedan',     1),
(33, 8,  N'Lexus RX',              N'SUV',       1),
(34, 8,  N'Lexus NX',              N'SUV',       0),
-- Hyundai (9)
(35, 9,  N'Hyundai Santa Fe',      N'SUV',       1),
(36, 9,  N'Hyundai Tucson',        N'SUV',       0),
(37, 9,  N'Hyundai Accent',        N'Sedan',     0),
(38, 9,  N'Hyundai Creta',         N'SUV',       0),
-- Kia (10)
(39, 10, N'Kia Sorento',           N'SUV',       1),
(40, 10, N'Kia Sportage',          N'SUV',       0),
(41, 10, N'Kia Cerato',            N'Sedan',     0),
(42, 10, N'Kia Morning',           N'Hatchback', 0),
-- Mazda (11)
(43, 11, N'Mazda CX-5',            N'SUV',       1),
(44, 11, N'Mazda CX-8',            N'SUV',       0),
(45, 11, N'Mazda3',                N'Sedan',     0),
(46, 11, N'Mazda6',                N'Sedan',     0),
-- Suzuki (12)
(47, 12, N'Suzuki Swift',          N'Hatchback', 0),
(48, 12, N'Suzuki Vitara',         N'SUV',       0),
(49, 12, N'Suzuki Ertiga',         N'MPV',       0),
-- Mitsubishi (13)
(50, 13, N'Mitsubishi Xpander',    N'MPV',       1),
(51, 13, N'Mitsubishi Outlander',  N'SUV',       0),
(52, 13, N'Mitsubishi Triton',     N'Bán tải',   0),
-- Nissan (14)
(53, 14, N'Nissan Navara',         N'Bán tải',   0),
(54, 14, N'Nissan Kicks',          N'SUV',       0),
(55, 14, N'Nissan Almera',         N'Sedan',     0),
-- Subaru (15)
(56, 15, N'Subaru Forester',       N'SUV',       0),
(57, 15, N'Subaru Outback',        N'SUV',       0),
(58, 15, N'Subaru XV',             N'SUV',       0);
SET IDENTITY_INSERT DongXe OFF;

-- ==================== 5. THÊM PHIÊN BẢN XE ====================
SET IDENTITY_INSERT PhienBanXe_SanPham ON;
INSERT INTO PhienBanXe_SanPham (MaPhienBan, MaDong, TenPhienBan, GiaNiemYet, MauSac, DongCo, HopSo, LoaiNhietLieu, SoLuongTrongKho, DuongDanAnh, MaKhuyenMai, TrangThai) VALUES
-- Camry (dong 1)
(1,  1,  N'Camry 2.0 CVT',                1050000000, N'Bạc',  N'2.0L 4 xi-lanh',       N'CVT',          N'Xăng',         5, '/images/cars/camry.jpg',       '', N'Còn hàng'),
(2,  1,  N'Camry 2.5 HEV',                1250000000, N'Đen',  N'2.5L Hybrid',          N'e-CVT',        N'Xăng + Điện',  3, '/images/cars/camry-hybrid.jpg','', N'Còn hàng'),
-- Hilux (dong 2)
(3,  2,  N'Hilux 2.4G 4x2 AT',            950000000,  N'Trắng',N'2.4L Turbo Diesel',    N'AT 6 cấp',     N'Dầu',          8, '/images/cars/hilux.jpg',       '', N'Còn hàng'),
(4,  2,  N'Hilux 2.8G 4x4 AT',            1150000000, N'Xám',  N'2.8L Turbo Diesel',    N'AT 6 cấp',     N'Dầu',          4, '/images/cars/hilux.jpg',       '', N'Còn hàng'),
-- Corolla Altis (dong 3)
(5,  3,  N'Corolla Altis 1.8G CVT',       750000000,  N'Đen',  N'1.8L 4 xi-lanh',       N'CVT',          N'Xăng',         6, '', '', N'Còn hàng'),
(6,  3,  N'Corolla Altis 1.8HEV',         850000000,  N'Xanh', N'1.8L Hybrid',          N'e-CVT',        N'Xăng + Điện',  2, '', '', N'Còn hàng'),
-- Fortuner (dong 4)
(7,  4,  N'Fortuner 2.4G 4x2 AT',         1100000000, N'Trắng',N'2.4L Turbo Diesel',    N'AT 6 cấp',     N'Dầu',          7, '', '', N'Còn hàng'),
(8,  4,  N'Fortuner 2.8V 4x4 AT',         1350000000, N'Đen',  N'2.8L Turbo Diesel',    N'AT 6 cấp',     N'Dầu',          3, '', '', N'Còn hàng'),
-- Vios (dong 5)
(9,  5,  N'Vios 1.5G CVT',                530000000,  N'Bạc',  N'1.5L 4 xi-lanh',       N'CVT',          N'Xăng',         15, '', '', N'Còn hàng'),
(10, 5,  N'Vios 1.5E MT',                 470000000,  N'Đỏ',   N'1.5L 4 xi-lanh',       N'MT 5 cấp',     N'Xăng',         10, '', '', N'Còn hàng'),
-- Civic (dong 6)
(11, 6,  N'Civic 1.5 Turbo CVT',          850000000,  N'Đỏ',   N'1.5L Turbo',           N'CVT',          N'Xăng',         6, '/images/cars/civic.jpg',       '', N'Còn hàng'),
(12, 6,  N'Civic RS 1.5 Turbo',           920000000,  N'Đen',  N'1.5L Turbo',           N'CVT',          N'Xăng',         3, '', '', N'Còn hàng'),
-- CR-V (dong 7)
(13, 7,  N'CR-V 1.5 Turbo G',             1050000000, N'Xanh', N'1.5L Turbo',           N'CVT',          N'Xăng',         4, '', '', N'Còn hàng'),
(14, 7,  N'CR-V 1.5 Turbo L',             1200000000, N'Xám',  N'1.5L Turbo',           N'CVT',          N'Xăng',         2, '', '', N'Còn hàng'),
-- HR-V (dong 8)
(15, 8,  N'HR-V 1.8L',                    750000000,  N'Trắng',N'1.8L 4 xi-lanh',       N'CVT',          N'Xăng',         5, '', '', N'Còn hàng'),
-- Accord (dong 9)
(16, 9,  N'Accord 2.0 Turbo',             1400000000, N'Đen',  N'2.0L Turbo',           N'CVT',          N'Xăng',         2, '', '', N'Còn hàng'),
-- Explorer (dong 10)
(17, 10, N'Explorer 2.3L EcoBoost',       1200000000, N'Xanh', N'2.3L EcoBoost',        N'AT 10 cấp',    N'Xăng',         4, '/images/cars/explorer.jpg',    '', N'Còn hàng'),
(18, 10, N'Explorer 3.0L V6',             1500000000, N'Đen',  N'3.0L V6 EcoBoost',     N'AT 10 cấp',    N'Xăng',         1, '', '', N'Còn hàng'),
-- Everest (dong 11)
(19, 11, N'Everest 2.0L Turbo 4x2',       1100000000, N'Trắng',N'2.0L Turbo Diesel',    N'AT 10 cấp',    N'Dầu',          5, '', '', N'Còn hàng'),
(20, 11, N'Everest 2.0L Turbo 4x4',       1300000000, N'Xám',  N'2.0L Turbo Diesel',    N'AT 10 cấp',    N'Dầu',          3, '', '', N'Còn hàng'),
-- Ranger (dong 12)
(21, 12, N'Ranger 2.0L XL 4x2',           700000000,  N'Trắng',N'2.0L Turbo Diesel',    N'AT 6 cấp',     N'Dầu',          10, '', '', N'Còn hàng'),
(22, 12, N'Ranger 2.0L Wildtrak 4x4',     950000000,  N'Đỏ',   N'2.0L Turbo Diesel',    N'AT 10 cấp',    N'Dầu',          6, '', '', N'Còn hàng'),
-- Territory (dong 13)
(23, 13, N'Territory 1.8L Titanium',      850000000,  N'Xanh', N'1.8L Turbo',           N'AT 7 cấp',     N'Xăng',         4, '', '', N'Còn hàng'),
-- 3 Series (dong 14)
(24, 14, N'320i Sport Line',              1600000000, N'Đen',  N'2.0L Turbo 4 xi-lanh', N'AT 8 cấp',     N'Xăng',         1, '/images/cars/bmw320.jpg',      '', N'Còn hàng'),
(25, 14, N'330i M Sport',                 1900000000, N'Xanh', N'2.0L Turbo 4 xi-lanh', N'AT 8 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
-- 5 Series (dong 15)
(26, 15, N'520i Luxury',                  2300000000, N'Bạc',  N'2.0L Turbo 4 xi-lanh', N'AT 8 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
(27, 15, N'530i M Sport',                 2700000000, N'Đen',  N'3.0L Turbo 6 xi-lanh', N'AT 8 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- X3 (dong 16)
(28, 16, N'X3 xDrive20i',                 2000000000, N'Trắng',N'2.0L Turbo 4 xi-lanh', N'AT 8 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
(29, 16, N'X3 M40i',                      2800000000, N'Xanh', N'3.0L Turbo 6 xi-lanh', N'AT 8 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- X5 (dong 17)
(30, 17, N'X5 xDrive40i',                 3500000000, N'Đen',  N'3.0L Turbo 6 xi-lanh', N'AT 8 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- Lux SA (dong 18)
(31, 18, N'Lux SA 2.0T Base',             1100000000, N'Trắng',N'2.0L Turbo',           N'AT 8 cấp',     N'Xăng',         7, '/images/cars/luxsa.jpg',       '', N'Còn hàng'),
(32, 18, N'Lux SA 2.0T Premium',          1300000000, N'Đen',  N'2.0L Turbo',           N'AT 8 cấp',     N'Xăng',         3, '', '', N'Còn hàng'),
-- Fadil (dong 19)
(33, 19, N'Fadil 1.2 Base',               360000000,  N'Đỏ',   N'1.2L 3 xi-lanh',       N'MT 5 cấp',     N'Xăng',         15, '/images/cars/fadil.jpg',       '', N'Còn hàng'),
(34, 19, N'Fadil 1.2 AT',                 395000000,  N'Bạc',  N'1.2L 3 xi-lanh',       N'CVT',          N'Xăng',         12, '', '', N'Còn hàng'),
-- VF e34 (dong 20)
(35, 20, N'VF e34 Eco',                   710000000,  N'Xanh', N'Điện động cơ 110kW',   N'1 cấp',        N'Điện',         5, '', '', N'Còn hàng'),
(36, 20, N'VF e34 Plus',                  770000000,  N'Trắng',N'Điện động cơ 110kW',   N'1 cấp',        N'Điện',         3, '', '', N'Còn hàng'),
-- VF 8 (dong 21)
(37, 21, N'VF 8 Eco',                     1050000000, N'Xanh', N'Điện động cơ 260kW',   N'1 cấp',        N'Điện',         4, '', '', N'Còn hàng'),
(38, 21, N'VF 8 Plus',                    1150000000, N'Đỏ',   N'Điện động cơ 300kW',   N'1 cấp',        N'Điện',         2, '', '', N'Còn hàng'),
-- VF 9 (dong 22)
(39, 22, N'VF 9 Eco',                     1600000000, N'Bạc',  N'Điện động cơ 300kW',   N'1 cấp',        N'Điện',         2, '', '', N'Còn hàng'),
(40, 22, N'VF 9 Plus',                    1800000000, N'Đen',  N'Điện động cơ 300kW',   N'1 cấp',        N'Điện',         1, '', '', N'Còn hàng'),
-- C-Class (dong 23)
(41, 23, N'C200 Avantgarde',              1500000000, N'Bạc',  N'1.5L Turbo + 48V',     N'AT 9 cấp',     N'Xăng',         3, '', '', N'Còn hàng'),
(42, 23, N'C300 AMG Line',                1850000000, N'Đen',  N'2.0L Turbo',           N'AT 9 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
-- E-Class (dong 24)
(43, 24, N'E200 Exclusive',               2100000000, N'Trắng',N'2.0L Turbo',           N'AT 9 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
(44, 24, N'E300 AMG Line',                2500000000, N'Xanh', N'2.0L Turbo',           N'AT 9 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- S-Class (dong 25)
(45, 25, N'S450L 4MATIC',                 5500000000, N'Đen',  N'3.0L Turbo 6 xi-lanh', N'AT 9 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- GLC (dong 26)
(46, 26, N'GLC 200 4MATIC',               1900000000, N'Xám',  N'2.0L Turbo',           N'AT 9 cấp',     N'Xăng',         3, '', '', N'Còn hàng'),
(47, 26, N'GLC 300 4MATIC',               2300000000, N'Trắng',N'2.0L Turbo',           N'AT 9 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
-- GLE (dong 27)
(48, 27, N'GLE 300d 4MATIC',              3000000000, N'Đen',  N'3.0L Turbo Diesel',    N'AT 9 cấp',     N'Dầu',          1, '', '', N'Còn hàng'),
(49, 27, N'GLE 450 4MATIC',               3700000000, N'Xanh', N'3.0L Turbo 6 xi-lanh', N'AT 9 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- A3 (dong 28)
(50, 28, N'A3 35 TFSI',                   1250000000, N'Bạc',  N'1.4L Turbo',           N'AT 7 cấp',     N'Xăng',         3, '', '', N'Còn hàng'),
-- A4 (dong 29)
(51, 29, N'A4 40 TFSI',                   1600000000, N'Đen',  N'2.0L Turbo',           N'AT 7 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
(52, 29, N'A4 45 TFSI Quattro',           1850000000, N'Trắng',N'2.0L Turbo',           N'AT 7 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- Q5 (dong 30)
(53, 30, N'Q5 40 TFSI',                   2200000000, N'Xanh', N'2.0L Turbo',           N'AT 7 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
-- Q7 (dong 31)
(54, 31, N'Q7 45 TFSI Quattro',           3200000000, N'Đen',  N'3.0L Turbo 6 xi-lanh', N'AT 8 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- ES (dong 32)
(55, 32, N'ES 250',                       2300000000, N'Bạc',  N'2.5L 4 xi-lanh',       N'AT 8 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
-- RX (dong 33)
(56, 33, N'RX 350 F Sport',               3300000000, N'Đen',  N'3.5L V6',              N'AT 8 cấp',     N'Xăng',         1, '', '', N'Còn hàng'),
-- NX (dong 34)
(57, 34, N'NX 250',                       2100000000, N'Xám',  N'2.5L 4 xi-lanh',       N'AT 8 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
(58, 34, N'NX 350h',                      2400000000, N'Xanh', N'2.5L Hybrid',          N'e-CVT',        N'Xăng + Điện',  1, '', '', N'Còn hàng'),
-- Santa Fe (dong 35)
(59, 35, N'Santa Fe 2.5 Premium',         1150000000, N'Xanh', N'2.5L 4 xi-lanh',       N'AT 6 cấp',     N'Xăng',         5, '', '', N'Còn hàng'),
(60, 35, N'Santa Fe 2.2D Calligraphy',    1350000000, N'Đen',  N'2.2L Turbo Diesel',    N'AT 8 cấp',     N'Dầu',          3, '', '', N'Còn hàng'),
-- Tucson (dong 36)
(61, 36, N'Tucson 2.0X Tiêu chuẩn',       750000000,  N'Trắng',N'2.0L 4 xi-lanh',       N'AT 6 cấp',     N'Xăng',         8, '', '', N'Còn hàng'),
(62, 36, N'Tucson 1.6T Đặc biệt',         900000000,  N'Đỏ',   N'1.6L Turbo',           N'AT 7 cấp',     N'Xăng',         4, '', '', N'Còn hàng'),
-- Accent (dong 37)
(63, 37, N'Accent 1.4MT Base',            430000000,  N'Bạc',  N'1.4L 4 xi-lanh',       N'MT 6 cấp',     N'Xăng',         12, '', '', N'Còn hàng'),
(64, 37, N'Accent 1.4AT Đặc biệt',        500000000,  N'Đen',  N'1.4L 4 xi-lanh',       N'AT 6 cấp',     N'Xăng',         8, '', '', N'Còn hàng'),
-- Creta (dong 38)
(65, 38, N'Creta 1.5 Tiêu chuẩn',         640000000,  N'Xanh', N'1.5L 4 xi-lanh',       N'MT 6 cấp',     N'Xăng',         6, '', '', N'Còn hàng'),
(66, 38, N'Creta 1.5 Đặc biệt',           720000000,  N'Trắng',N'1.5L 4 xi-lanh',       N'CVT',          N'Xăng',         4, '', '', N'Còn hàng'),
-- Sorento (dong 39)
(67, 39, N'Sorento 2.5 X-Line',           1250000000, N'Đen',  N'2.5L 4 xi-lanh',       N'AT 6 cấp',     N'Xăng',         3, '', '', N'Còn hàng'),
(68, 39, N'Sorento 2.2D Premium',         1450000000, N'Xám',  N'2.2L Turbo Diesel',    N'AT 8 cấp',     N'Dầu',          2, '', '', N'Còn hàng'),
-- Sportage (dong 40)
(69, 40, N' Sportage 2.0 Tiêu chuẩn',     800000000,  N'Trắng',N'2.0L 4 xi-lanh',       N'AT 6 cấp',     N'Xăng',         5, '', '', N'Còn hàng'),
(70, 40, N' Sportage 1.6T GT-Line',       1000000000, N'Xanh', N'1.6L Turbo',           N'AT 7 cấp',     N'Xăng',         3, '', '', N'Còn hàng'),
-- Cerato (dong 41)
(71, 41, N'Cerato 1.6MT',                 580000000,  N'Bạc',  N'1.6L 4 xi-lanh',       N'MT 6 cấp',     N'Xăng',         7, '', '', N'Còn hàng'),
(72, 41, N'Cerato 2.0AT',                 680000000,  N'Đỏ',   N'2.0L 4 xi-lanh',       N'AT 6 cấp',     N'Xăng',         5, '', '', N'Còn hàng'),
-- Morning (dong 42)
(73, 42, N'Morning 1.25MT',               360000000,  N'Đỏ',   N'1.25L 4 xi-lanh',      N'MT 5 cấp',     N'Xăng',         18, '', '', N'Còn hàng'),
(74, 42, N'Morning 1.25AT',               400000000,  N'Trắng',N'1.25L 4 xi-lanh',      N'AT 4 cấp',     N'Xăng',         14, '', '', N'Còn hàng'),
-- CX-5 (dong 43)
(75, 43, N'CX-5 2.0L Deluxe',             850000000,  N'Đỏ',   N'2.0L SkyActiv',       N'AT 6 cấp',     N'Xăng',         6, '', '', N'Còn hàng'),
(76, 43, N'CX-5 2.5L Signature',          1050000000, N'Xanh', N'2.5L SkyActiv',       N'AT 6 cấp',     N'Xăng',         4, '', '', N'Còn hàng'),
-- CX-8 (dong 44)
(77, 44, N'CX-8 2.5L Premium',           1150000000, N'Đen',  N'2.5L SkyActiv',       N'AT 6 cấp',     N'Xăng',         3, '', '', N'Còn hàng'),
-- Mazda3 (dong 45)
(78, 45, N'Mazda3 1.5L Deluxe',          620000000,  N'Bạc',  N'1.5L SkyActiv',       N'AT 6 cấp',     N'Xăng',         8, '', '', N'Còn hàng'),
(79, 45, N'Mazda3 2.0L Premium',         720000000,  N'Đỏ',   N'2.0L SkyActiv',       N'AT 6 cấp',     N'Xăng',         5, '', '', N'Còn hàng'),
-- Mazda6 (dong 46)
(80, 46, N'Mazda6 2.0L Deluxe',          850000000,  N'Xám',  N'2.0L SkyActiv',       N'AT 6 cấp',     N'Xăng',         4, '', '', N'Còn hàng'),
(81, 46, N'Mazda6 2.5L Premium',         1000000000, N'Đen',  N'2.5L SkyActiv',       N'AT 6 cấp',     N'Xăng',         2, '', '', N'Còn hàng'),
-- Swift (dong 47)
(82, 47, N'Swift 1.2L AT',               480000000,  N'Xanh', N'1.2L 4 xi-lanh',       N'CVT',          N'Xăng',         6, '', '', N'Còn hàng'),
-- Vitara (dong 48)
(83, 48, N'Vitara 1.6L AT',              680000000,  N'Xám',  N'1.6L 4 xi-lanh',       N'AT 6 cấp',     N'Xăng',         4, '', '', N'Còn hàng'),
-- Ertiga (dong 49)
(84, 49, N'Ertiga 1.5L AT',              580000000,  N'Trắng',N'1.5L 4 xi-lanh',       N'AT 4 cấp',     N'Xăng',         5, '', '', N'Còn hàng'),
-- Xpander (dong 50)
(85, 50, N'Xpander 1.5L AT',             660000000,  N'Bạc',  N'1.5L 4 xi-lanh',       N'AT 4 cấp',     N'Xăng',         7, '', '', N'Còn hàng'),
-- Outlander (dong 51)
(86, 51, N'Outlander 2.0L CVT',          950000000,  N'Xanh', N'2.0L 4 xi-lanh',       N'CVT',          N'Xăng',         3, '', '', N'Còn hàng'),
-- Triton (dong 52)
(87, 52, N'Triton 2.4L 4x2 AT',          720000000,  N'Trắng',N'2.4L Turbo Diesel',    N'AT 5 cấp',     N'Dầu',          6, '', '', N'Còn hàng'),
(88, 52, N'Triton 2.4L 4x4 AT',          850000000,  N'Đen',  N'2.4L Turbo Diesel',    N'AT 5 cấp',     N'Dầu',          4, '', '', N'Còn hàng'),
-- Navara (dong 53)
(89, 53, N'Navara 2.5L 4x2 AT',          750000000,  N'Trắng',N'2.5L Turbo Diesel',    N'AT 7 cấp',     N'Dầu',          5, '', '', N'Còn hàng'),
(90, 53, N'Navara 2.5L 4x4 AT',          950000000,  N'Đỏ',   N'2.5L Turbo Diesel',    N'AT 7 cấp',     N'Dầu',          3, '', '', N'Còn hàng'),
-- Kicks (dong 54)
(91, 54, N'Kicks 1.2L AT',               680000000,  N'Xanh', N'1.2L 3 xi-lanh',       N'CVT',          N'Xăng',         4, '', '', N'Còn hàng'),
-- Almera (dong 55)
(92, 55, N'Almera 1.5L AT',              550000000,  N'Bạc',  N'1.5L 4 xi-lanh',       N'CVT',          N'Xăng',         6, '', '', N'Còn hàng'),
-- Forester (dong 56)
(93, 56, N'Forester 2.0L i-L',           1150000000, N'Xanh', N'2.0L 4 xi-lanh',       N'CVT',          N'Xăng',         2, '', '', N'Còn hàng'),
(94, 56, N'Forester 2.0L i-S',           1300000000, N'Xám',  N'2.0L 4 xi-lanh',       N'CVT',          N'Xăng',         1, '', '', N'Còn hàng'),
-- Outback (dong 57)
(95, 57, N'Outback 2.5L',                1700000000, N'Đen',  N'2.5L 4 xi-lanh',       N'CVT',          N'Xăng',         1, '', '', N'Còn hàng'),
-- XV (dong 58)
(96, 58, N'XV 2.0L',                     900000000,  N'Xanh', N'2.0L 4 xi-lanh',       N'CVT',          N'Xăng',         2, '', '', N'Còn hàng');
SET IDENTITY_INSERT PhienBanXe_SanPham OFF;

-- ==================== 6. THÊM CHI NHÁNH ====================
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN01', N'Showroom Sài Gòn - Quận 7', N'123 Nguyễn Văn Linh, P. Tân Phong, Quận 7', N'TP. Hồ Chí Minh', '0909123456', 'quanly1', N'Hoạt động');
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN02', N'Showroom Sài Gòn - Thủ Đức', N'456 Xa lộ Hà Nội, P. Bình Thọ, TP. Thủ Đức', N'TP. Hồ Chí Minh', '0911222333', 'quanly2', N'Hoạt động');
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN03', N'Showroom Hà Nội - Cầu Giấy', N'789 Trần Duy Hưng, P. Trung Hòa, Q. Cầu Giấy', N'Hà Nội', '0922333444', 'quanly3', N'Hoạt động');
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN04', N'Showroom Hà Nội - Hoàng Mai', N'321 Giải Phóng, P. Hoàng Văn Thụ, Q. Hoàng Mai', N'Hà Nội', '0933444555', 'quanly4', N'Hoạt động');
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN05', N'Showroom Đà Nẵng', N'654 Nguyễn Văn Linh, P. Khuê Trung, Q. Hải Châu', N'Đà Nẵng', '0944555666', 'quanly5', N'Hoạt động');
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN06', N'Showroom Hải Phòng', N'987 Võ Nguyên Giáp, P. Vĩnh Niệm, Q. Lê Chân', N'Hải Phòng', '0955666777', 'quanly6', N'Hoạt động');

-- ==================== 5b. TỒN KHO THEO CHI NHÁNH ====================
SET IDENTITY_INSERT TonKhoTheoChiNhanh ON;
INSERT INTO TonKhoTheoChiNhanh (MaTonKho, MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES
(1002, 1, 'CN01', 1, '2026-07-29 11:37:59'),
(1003, 1, 'CN02', 1, '2026-07-29 11:37:59'),
(1004, 1, 'CN03', 1, '2026-07-29 11:37:59'),
(1005, 1, 'CN06', 2, '2026-07-29 11:37:59'),
(1006, 2, 'CN01', 1, '2026-07-29 11:37:59'),
(1007, 2, 'CN06', 2, '2026-07-29 11:37:59'),
(1008, 3, 'CN01', 2, '2026-07-29 11:37:59'),
(1009, 3, 'CN02', 1, '2026-07-29 11:37:59'),
(1010, 3, 'CN03', 1, '2026-07-29 11:37:59'),
(1011, 3, 'CN06', 3, '2026-07-29 11:37:59'),
(1012, 4, 'CN01', 1, '2026-07-29 11:37:59'),
(1013, 4, 'CN02', 1, '2026-07-29 11:37:59'),
(1014, 4, 'CN06', 2, '2026-07-29 11:37:59'),
(1015, 5, 'CN01', 2, '2026-07-29 11:37:59'),
(1016, 5, 'CN02', 1, '2026-07-29 11:37:59'),
(1017, 5, 'CN03', 1, '2026-07-29 11:37:59'),
(1018, 5, 'CN06', 2, '2026-07-29 11:37:59'),
(1019, 6, 'CN06', 2, '2026-07-29 11:37:59'),
(1020, 7, 'CN01', 2, '2026-07-29 11:37:59'),
(1021, 7, 'CN02', 1, '2026-07-29 11:37:59'),
(1022, 7, 'CN03', 1, '2026-07-29 11:37:59'),
(1023, 7, 'CN06', 3, '2026-07-29 11:37:59'),
(1024, 8, 'CN01', 1, '2026-07-29 11:37:59'),
(1025, 8, 'CN06', 2, '2026-07-29 11:37:59'),
(1026, 9, 'CN01', 5, '2026-07-29 11:37:59'),
(1027, 9, 'CN02', 3, '2026-07-29 11:37:59'),
(1028, 9, 'CN03', 3, '2026-07-29 11:37:59'),
(1029, 9, 'CN04', 1, '2026-07-29 11:37:59'),
(1030, 9, 'CN06', 3, '2026-07-29 11:37:59'),
(1031, 10, 'CN01', 3, '2026-07-29 11:37:59'),
(1032, 10, 'CN02', 2, '2026-07-29 11:37:59'),
(1033, 10, 'CN03', 2, '2026-07-29 11:37:59'),
(1034, 10, 'CN04', 1, '2026-07-29 11:37:59'),
(1035, 10, 'CN06', 2, '2026-07-29 11:37:59'),
(1036, 11, 'CN01', 2, '2026-07-29 11:37:59'),
(1037, 11, 'CN02', 1, '2026-07-29 11:37:59'),
(1038, 11, 'CN03', 1, '2026-07-29 11:37:59'),
(1039, 11, 'CN06', 2, '2026-07-29 11:37:59'),
(1040, 12, 'CN01', 1, '2026-07-29 11:37:59'),
(1041, 12, 'CN06', 2, '2026-07-29 11:37:59'),
(1042, 13, 'CN01', 1, '2026-07-29 11:37:59'),
(1043, 13, 'CN02', 1, '2026-07-29 11:37:59'),
(1044, 13, 'CN06', 2, '2026-07-29 11:37:59'),
(1045, 14, 'CN06', 2, '2026-07-29 11:37:59'),
(1046, 15, 'CN01', 1, '2026-07-29 11:37:59'),
(1047, 15, 'CN02', 1, '2026-07-29 11:37:59'),
(1048, 15, 'CN03', 1, '2026-07-29 11:37:59'),
(1049, 15, 'CN06', 2, '2026-07-29 11:37:59'),
(1050, 16, 'CN06', 2, '2026-07-29 11:37:59'),
(1051, 17, 'CN01', 1, '2026-07-29 11:37:59'),
(1052, 17, 'CN02', 1, '2026-07-29 11:37:59'),
(1053, 17, 'CN06', 2, '2026-07-29 11:37:59'),
(1054, 18, 'CN06', 1, '2026-07-29 11:37:59'),
(1055, 19, 'CN01', 1, '2026-07-29 11:37:59'),
(1056, 19, 'CN02', 1, '2026-07-29 11:37:59'),
(1057, 19, 'CN03', 1, '2026-07-29 11:37:59'),
(1058, 19, 'CN06', 2, '2026-07-29 11:37:59'),
(1059, 20, 'CN01', 1, '2026-07-29 11:37:59'),
(1060, 20, 'CN06', 2, '2026-07-29 11:37:59'),
(1061, 21, 'CN01', 3, '2026-07-29 11:37:59'),
(1062, 21, 'CN02', 2, '2026-07-29 11:37:59'),
(1063, 21, 'CN03', 2, '2026-07-29 11:37:59'),
(1064, 21, 'CN04', 1, '2026-07-29 11:37:59'),
(1065, 21, 'CN06', 2, '2026-07-29 11:37:59'),
(1066, 22, 'CN01', 2, '2026-07-29 11:37:59'),
(1067, 22, 'CN02', 1, '2026-07-29 11:37:59'),
(1068, 22, 'CN03', 1, '2026-07-29 11:37:59'),
(1069, 22, 'CN06', 2, '2026-07-29 11:37:59'),
(1070, 23, 'CN01', 1, '2026-07-29 11:37:59'),
(1071, 23, 'CN02', 1, '2026-07-29 11:37:59'),
(1072, 23, 'CN06', 2, '2026-07-29 11:37:59'),
(1073, 25, 'CN06', 2, '2026-07-29 11:37:59'),
(1074, 26, 'CN06', 1, '2026-07-29 11:37:59'),
(1075, 27, 'CN06', 1, '2026-07-29 11:37:59'),
(1076, 28, 'CN06', 2, '2026-07-29 11:37:59'),
(1077, 29, 'CN06', 1, '2026-07-29 11:37:59'),
(1078, 30, 'CN06', 1, '2026-07-29 11:37:59'),
(1079, 31, 'CN01', 2, '2026-07-29 11:37:59'),
(1080, 31, 'CN02', 1, '2026-07-29 11:37:59'),
(1081, 31, 'CN03', 1, '2026-07-29 11:37:59'),
(1082, 31, 'CN06', 2, '2026-07-29 11:37:59'),
(1083, 32, 'CN01', 1, '2026-07-29 11:37:59'),
(1084, 32, 'CN06', 2, '2026-07-29 11:37:59'),
(1085, 33, 'CN01', 5, '2026-07-29 11:37:59'),
(1086, 33, 'CN02', 3, '2026-07-29 11:37:59'),
(1087, 33, 'CN03', 3, '2026-07-29 11:37:59'),
(1088, 33, 'CN04', 1, '2026-07-29 11:37:59'),
(1089, 33, 'CN06', 3, '2026-07-29 11:37:59'),
(1090, 34, 'CN01', 4, '2026-07-29 11:37:59'),
(1091, 34, 'CN02', 3, '2026-07-29 11:37:59'),
(1092, 34, 'CN03', 2, '2026-07-29 11:37:59'),
(1093, 34, 'CN04', 1, '2026-07-29 11:37:59'),
(1094, 34, 'CN06', 2, '2026-07-29 11:37:59'),
(1095, 35, 'CN01', 1, '2026-07-29 11:37:59'),
(1096, 35, 'CN02', 1, '2026-07-29 11:37:59'),
(1097, 35, 'CN03', 1, '2026-07-29 11:37:59'),
(1098, 35, 'CN06', 2, '2026-07-29 11:37:59'),
(1099, 36, 'CN01', 1, '2026-07-29 11:37:59'),
(1100, 36, 'CN06', 2, '2026-07-29 11:37:59'),
(1101, 37, 'CN01', 1, '2026-07-29 11:37:59'),
(1102, 37, 'CN06', 2, '2026-07-29 11:37:59'),
(1103, 38, 'CN06', 2, '2026-07-29 11:37:59'),
(1104, 39, 'CN06', 2, '2026-07-29 11:37:59'),
(1105, 40, 'CN06', 1, '2026-07-29 11:37:59'),
(1106, 41, 'CN06', 2, '2026-07-29 11:37:59'),
(1107, 42, 'CN06', 2, '2026-07-29 11:37:59'),
(1108, 43, 'CN06', 2, '2026-07-29 11:37:59'),
(1109, 44, 'CN06', 1, '2026-07-29 11:37:59'),
(1110, 45, 'CN06', 1, '2026-07-29 11:37:59'),
(1111, 46, 'CN01', 1, '2026-07-29 11:37:59'),
(1112, 46, 'CN06', 2, '2026-07-29 11:37:59'),
(1113, 47, 'CN06', 2, '2026-07-29 11:37:59'),
(1114, 48, 'CN06', 1, '2026-07-29 11:37:59'),
(1115, 49, 'CN06', 1, '2026-07-29 11:37:59'),
(1116, 50, 'CN01', 1, '2026-07-29 11:37:59'),
(1117, 50, 'CN06', 2, '2026-07-29 11:37:59'),
(1118, 51, 'CN06', 2, '2026-07-29 11:37:59'),
(1119, 52, 'CN06', 1, '2026-07-29 11:37:59'),
(1120, 53, 'CN06', 2, '2026-07-29 11:37:59'),
(1121, 54, 'CN06', 1, '2026-07-29 11:37:59'),
(1122, 55, 'CN06', 2, '2026-07-29 11:37:59'),
(1123, 56, 'CN06', 1, '2026-07-29 11:37:59'),
(1124, 57, 'CN06', 2, '2026-07-29 11:37:59'),
(1125, 58, 'CN06', 1, '2026-07-29 11:37:59'),
(1126, 59, 'CN01', 1, '2026-07-29 11:37:59'),
(1127, 59, 'CN02', 1, '2026-07-29 11:37:59'),
(1128, 59, 'CN06', 2, '2026-07-29 11:37:59'),
(1129, 60, 'CN01', 1, '2026-07-29 11:37:59'),
(1130, 60, 'CN06', 2, '2026-07-29 11:37:59'),
(1131, 61, 'CN01', 2, '2026-07-29 11:37:59'),
(1132, 61, 'CN02', 2, '2026-07-29 11:37:59'),
(1133, 61, 'CN03', 1, '2026-07-29 11:37:59'),
(1134, 61, 'CN06', 3, '2026-07-29 11:37:59'),
(1135, 62, 'CN01', 1, '2026-07-29 11:37:59'),
(1136, 62, 'CN02', 1, '2026-07-29 11:37:59'),
(1137, 62, 'CN06', 2, '2026-07-29 11:37:59'),
(1138, 63, 'CN01', 4, '2026-07-29 11:37:59'),
(1139, 63, 'CN02', 3, '2026-07-29 11:37:59'),
(1140, 63, 'CN03', 2, '2026-07-29 11:37:59'),
(1141, 63, 'CN04', 1, '2026-07-29 11:37:59'),
(1142, 63, 'CN06', 2, '2026-07-29 11:37:59'),
(1143, 64, 'CN01', 2, '2026-07-29 11:37:59'),
(1144, 64, 'CN02', 2, '2026-07-29 11:37:59'),
(1145, 64, 'CN03', 1, '2026-07-29 11:37:59'),
(1146, 64, 'CN06', 3, '2026-07-29 11:37:59'),
(1147, 65, 'CN01', 2, '2026-07-29 11:37:59'),
(1148, 65, 'CN02', 1, '2026-07-29 11:37:59'),
(1149, 65, 'CN03', 1, '2026-07-29 11:37:59'),
(1150, 65, 'CN06', 2, '2026-07-29 11:37:59'),
(1151, 66, 'CN01', 1, '2026-07-29 11:37:59'),
(1152, 66, 'CN02', 1, '2026-07-29 11:37:59'),
(1153, 66, 'CN06', 2, '2026-07-29 11:37:59'),
(1154, 67, 'CN01', 1, '2026-07-29 11:37:59'),
(1155, 67, 'CN06', 2, '2026-07-29 11:37:59'),
(1156, 68, 'CN06', 2, '2026-07-29 11:37:59'),
(1157, 69, 'CN01', 1, '2026-07-29 11:37:59'),
(1158, 69, 'CN02', 1, '2026-07-29 11:37:59'),
(1159, 69, 'CN03', 1, '2026-07-29 11:37:59'),
(1160, 69, 'CN06', 2, '2026-07-29 11:37:59'),
(1161, 70, 'CN01', 1, '2026-07-29 11:37:59'),
(1162, 70, 'CN06', 2, '2026-07-29 11:37:59'),
(1163, 71, 'CN01', 2, '2026-07-29 11:37:59'),
(1164, 71, 'CN02', 1, '2026-07-29 11:37:59'),
(1165, 71, 'CN03', 1, '2026-07-29 11:37:59'),
(1166, 71, 'CN06', 3, '2026-07-29 11:37:59'),
(1167, 72, 'CN01', 1, '2026-07-29 11:37:59'),
(1168, 72, 'CN02', 1, '2026-07-29 11:37:59'),
(1169, 72, 'CN03', 1, '2026-07-29 11:37:59'),
(1170, 72, 'CN06', 2, '2026-07-29 11:37:59'),
(1171, 73, 'CN01', 6, '2026-07-29 11:37:59'),
(1172, 73, 'CN02', 4, '2026-07-29 11:37:59'),
(1173, 73, 'CN03', 3, '2026-07-29 11:37:59'),
(1174, 73, 'CN04', 1, '2026-07-29 11:37:59'),
(1175, 73, 'CN06', 4, '2026-07-29 11:37:59'),
(1176, 74, 'CN01', 4, '2026-07-29 11:37:59'),
(1177, 74, 'CN02', 3, '2026-07-29 11:37:59'),
(1178, 74, 'CN03', 2, '2026-07-29 11:37:59'),
(1179, 74, 'CN04', 1, '2026-07-29 11:37:59'),
(1180, 74, 'CN06', 4, '2026-07-29 11:37:59'),
(1181, 75, 'CN01', 2, '2026-07-29 11:37:59'),
(1182, 75, 'CN02', 1, '2026-07-29 11:37:59'),
(1183, 75, 'CN03', 1, '2026-07-29 11:37:59'),
(1184, 75, 'CN06', 2, '2026-07-29 11:37:59'),
(1185, 76, 'CN01', 1, '2026-07-29 11:37:59'),
(1186, 76, 'CN02', 1, '2026-07-29 11:37:59'),
(1187, 76, 'CN06', 2, '2026-07-29 11:37:59'),
(1188, 77, 'CN01', 1, '2026-07-29 11:37:59'),
(1189, 77, 'CN06', 2, '2026-07-29 11:37:59'),
(1190, 78, 'CN01', 2, '2026-07-29 11:37:59'),
(1191, 78, 'CN02', 2, '2026-07-29 11:37:59'),
(1192, 78, 'CN03', 1, '2026-07-29 11:37:59'),
(1193, 78, 'CN06', 3, '2026-07-29 11:37:59'),
(1194, 79, 'CN01', 1, '2026-07-29 11:37:59'),
(1195, 79, 'CN02', 1, '2026-07-29 11:37:59'),
(1196, 79, 'CN03', 1, '2026-07-29 11:37:59'),
(1197, 79, 'CN06', 2, '2026-07-29 11:37:59'),
(1198, 80, 'CN01', 1, '2026-07-29 11:37:59'),
(1199, 80, 'CN02', 1, '2026-07-29 11:37:59'),
(1200, 80, 'CN06', 2, '2026-07-29 11:37:59'),
(1201, 81, 'CN06', 2, '2026-07-29 11:37:59'),
(1202, 82, 'CN01', 2, '2026-07-29 11:37:59'),
(1203, 82, 'CN02', 1, '2026-07-29 11:37:59'),
(1204, 82, 'CN03', 1, '2026-07-29 11:37:59'),
(1205, 82, 'CN06', 2, '2026-07-29 11:37:59'),
(1206, 83, 'CN01', 1, '2026-07-29 11:37:59'),
(1207, 83, 'CN02', 1, '2026-07-29 11:37:59'),
(1208, 83, 'CN06', 2, '2026-07-29 11:37:59'),
(1209, 84, 'CN01', 1, '2026-07-29 11:37:59'),
(1210, 84, 'CN02', 1, '2026-07-29 11:37:59'),
(1211, 84, 'CN03', 1, '2026-07-29 11:37:59'),
(1212, 84, 'CN06', 2, '2026-07-29 11:37:59'),
(1213, 85, 'CN01', 2, '2026-07-29 11:37:59'),
(1214, 85, 'CN02', 1, '2026-07-29 11:37:59'),
(1215, 85, 'CN03', 1, '2026-07-29 11:37:59'),
(1216, 85, 'CN06', 3, '2026-07-29 11:37:59'),
(1217, 86, 'CN01', 1, '2026-07-29 11:37:59'),
(1218, 86, 'CN06', 2, '2026-07-29 11:37:59'),
(1219, 87, 'CN01', 2, '2026-07-29 11:37:59'),
(1220, 87, 'CN02', 1, '2026-07-29 11:37:59'),
(1221, 87, 'CN03', 1, '2026-07-29 11:37:59'),
(1222, 87, 'CN06', 2, '2026-07-29 11:37:59'),
(1223, 88, 'CN01', 1, '2026-07-29 11:37:59'),
(1224, 88, 'CN02', 1, '2026-07-29 11:37:59'),
(1225, 88, 'CN06', 2, '2026-07-29 11:37:59'),
(1226, 89, 'CN01', 1, '2026-07-29 11:37:59'),
(1227, 89, 'CN02', 1, '2026-07-29 11:37:59'),
(1228, 89, 'CN03', 1, '2026-07-29 11:37:59'),
(1229, 89, 'CN06', 2, '2026-07-29 11:37:59'),
(1230, 90, 'CN01', 1, '2026-07-29 11:37:59'),
(1231, 90, 'CN06', 2, '2026-07-29 11:37:59'),
(1232, 91, 'CN01', 1, '2026-07-29 11:37:59'),
(1233, 91, 'CN02', 1, '2026-07-29 11:37:59'),
(1234, 91, 'CN06', 2, '2026-07-29 11:37:59'),
(1235, 92, 'CN01', 2, '2026-07-29 11:37:59'),
(1236, 92, 'CN02', 1, '2026-07-29 11:37:59'),
(1237, 92, 'CN03', 1, '2026-07-29 11:37:59'),
(1238, 92, 'CN06', 2, '2026-07-29 11:37:59'),
(1239, 93, 'CN06', 2, '2026-07-29 11:37:59'),
(1240, 94, 'CN06', 1, '2026-07-29 11:37:59'),
(1241, 95, 'CN06', 1, '2026-07-29 11:37:59'),
(1242, 96, 'CN06', 2, '2026-07-29 11:37:59');
SET IDENTITY_INSERT TonKhoTheoChiNhanh OFF;

-- ==================== 7. THÊM KHUYẾN MÃI ====================
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM01', N'Giảm 50% lệ phí trước bạ', N'Áp dụng cho tất cả dòng xe', N'Phần trăm', 50, 50000000, '2024-01-01', '2024-03-31', N'Hoạt động');
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM02', N'Giảm ngay 20 triệu', N'Cho dòng xe Toyota Vios và Hyundai Accent', N'Số tiền', 20000000, 20000000, '2026-07-01', '2026-09-30', N'Hoạt động');
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM03', N'Tặng gói phụ kiện 15 triệu', N'Cho khách đặt cọc xe Mercedes trước 30/09', N'Số tiền', 15000000, 15000000, '2026-07-01', '2026-09-30', N'Hoạt động');
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM04', N'Giảm 10% cho xe điện VinFast', N'Áp dụng cho VF e34, VF 8, VF 9', N'Phần trăm', 10, 150000000, '2026-08-01', '2026-12-31', N'Hoạt động');

-- ==================== 8. THÊM BANNER ====================
SET IDENTITY_INSERT QuangCaoBanner ON;
INSERT INTO QuangCaoBanner (MaBanner, DuongDanAnh, DuongDanLienKet, ThuTuHienThi, MaQuanLyCapNhat, TrangThaiKichHoat) VALUES
(1, '/images/banners/banner1.jpg', '/Details/1',  1, 'fntzzs682@gmail.com', 1),
(2, '/images/banners/banner2.jpg', '/Details/31', 2, 'fntzzs682@gmail.com', 1),
(3, '/images/banners/banner3.jpg', '/Details/24', 3, 'fntzzs682@gmail.com', 1),
(4, '/images/banners/banner4.jpg', '/Details/41', 4, 'fntzzs682@gmail.com', 1),
(5, '/images/banners/banner5.jpg', '/Details/37', 5, 'fntzzs682@gmail.com', 1);
SET IDENTITY_INSERT QuangCaoBanner OFF;

-- ==================== 9. THÊM KÊNH TƯ VẤN ====================
SET IDENTITY_INSERT KenhTuVan ON;
INSERT INTO KenhTuVan (MaKenh, UrlMessenger, UrlZalo, UrlSMS) VALUES
(1, 'https://m.me/carshop', 'https://zalo.me/carshop', '0906123456');
SET IDENTITY_INSERT KenhTuVan OFF;

-- ==================== 10. THÊM ĐƠN CỌC MẪU ====================
SET IDENTITY_INSERT DonDatCoc ON;
-- Đã giao xe (MaDonCoc 1-6)
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (1, 'user1', 31, 'quanly1', 'CN01', 200000000, N'Chuyển khoản', N'Đã thanh toán', '2026-01-15 09:30:00', '2026-02-20', N'Đã giao xe', N'Giao tại showroom Quận 7', N'Nguyễn Văn A', '0901000001', N'123 Lê Lợi, Quận 1', 'MGC250701-1');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (2, 'user2', 3,  'quanly2', 'CN02', 300000000, N'Chuyển khoản', N'Đã thanh toán', '2026-02-10 14:00:00', '2026-03-05', N'Đã giao xe', N'Xe màu bạc', N'Trần Văn B', '0901000002', N'456 Nguyễn Huệ, Quận 1', 'MGC250702-2');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (3, 'user3', 59, 'quanly3', 'CN03', 150000000, N'Tiền mặt', N'Đã thanh toán', '2026-03-05 10:00:00', '2026-03-28', N'Đã giao xe', N'Giao tại showroom Cầu Giấy', N'Lê Thị C', '0901000003', N'789 Trần Hưng Đạo, Hoàn Kiếm', 'MGC250703-3');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (4, 'user4', 24, 'quanly4', 'CN04', 450000000, N'Chuyển khoản', N'Đã thanh toán', '2026-03-20 15:30:00', '2026-04-15', N'Đã giao xe', N'Khách VIP - gói phụ kiện', N'Phạm Văn D', '0901000004', N'123 Nguyễn Văn Linh, Quận 7', 'MGC250704-4');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (5, 'user5', 41, 'quanly5', 'CN05', 300000000, N'Chuyển khoản', N'Đã thanh toán', '2026-03-25 08:00:00', '2026-04-20', N'Đã giao xe', N'Mercedes C200 màu bạc', N'Đỗ Thúy Hằng', '0901000011', N'456 Nguyễn Văn Linh, Đà Nẵng', 'MGC250711-11');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (6, 'user1', 37, 'quanly6', 'CN06', 250000000, N'Chuyển khoản', N'Đã thanh toán', '2026-04-05 11:00:00', '2026-05-10', N'Đã giao xe', N'VF 8 màu xanh', N'Nguyễn Văn G', '0901000012', N'789 Văn Cao, Hải Phòng', 'MGC250712-12');
-- Đã xác nhận (MaDonCoc 7-9)
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (7, 'user2', 2,  'quanly1', 'CN01', 250000000, N'Chuyển khoản', N'Đã thanh toán', '2026-04-10 09:00:00', '2026-05-20', N'Đã xác nhận', N'Camry Hybrid màu đen', N'Hoàng Thị E', '0901000005', N'456 Hải Phòng, Đà Nẵng', 'MGC250705-5');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (8, 'user3', 11, 'quanly2', 'CN02', 180000000, N'Tiền mặt', N'Đã thanh toán', '2026-05-05 11:30:00', '2026-06-10', N'Đã xác nhận', N'Civic Turbo màu đỏ', N'Đặng Văn F', '0901000006', N'789 Văn Cao, Hải Phòng', 'MGC250706-6');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (9, 'user4', 75, 'quanly3', 'CN03', 100000000, N'Chuyển khoản', N'Đã thanh toán', '2026-06-20 14:00:00', '2026-07-25', N'Đã xác nhận', N'CX-5 Deluxe màu đỏ', N'Vũ Thị M', '0901000013', N'123 Hoàng Quốc Việt, Cầu Giấy', 'MGC250713-13');
-- Chờ xác nhận (MaDonCoc 10-12)
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (10, 'user5', 17, 'quanly1', 'CN01', 200000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-06-01 08:00:00', N'Chờ xác nhận', N'Khách đang chờ vay ngân hàng', N'Nguyễn Văn H', '0901000007', N'123 Quận 7, TP.HCM', 'MGC250707-7');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (11, 'user1', 46, 'quanly2', 'CN02', 350000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-06-15 14:00:00', N'Chờ xác nhận', N'Khách muốn lái thử trước', N'Trần Thị K', '0901000008', N'456 Thủ Đức, TP.HCM', 'MGC250708-8');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (12, 'user2', 67, 'quanly3', 'CN03', 150000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-07-01 09:00:00', N'Chờ xác nhận', N'Đang thương lượng giá', N'Lê Văn P', '0901000014', N'789 Cầu Giấy, Hà Nội', 'MGC250714-14');
-- Đã hủy (MaDonCoc 13-15)
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (13, 'user3', 33, 'quanly4', 'CN04', 100000000, N'Tiền mặt', N'Đã hoàn tiền', '2026-04-20 16:00:00', N'Đã hủy', N'Khách đổi ý không mua nữa', N'Lê Văn I', '0901000009', N'789 Cầu Giấy, Hà Nội', 'MGC250709-9');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (14, 'user4', 9,  'quanly5', 'CN05', 150000000, N'Chuyển khoản', N'Đã hoàn tiền', '2026-05-10 10:30:00', N'Đã hủy', N'Không đủ khả năng tài chính', N'Phạm Thị K', '0901000010', N'321 Hoàng Mai, Hà Nội', 'MGC250710-10');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, MaChiNhanh, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (15, 'user5', 61, 'quanly6', 'CN06', 80000000, N'Chuyển khoản', N'Đã hoàn tiền', '2026-06-25 15:00:00', N'Đã hủy', N'Chọn mua dòng xe khác', N'Ngô Văn Q', '0901000015', N'456 Lê Lợi, Hải Phòng', 'MGC250715-15');
SET IDENTITY_INSERT DonDatCoc OFF;

-- ==================== 10b. CHI TIẾT ĐƠN CỌC ====================
SET IDENTITY_INSERT DonDatCocChiTiet ON;
INSERT INTO DonDatCocChiTiet (MaChiTiet, MaDonCoc, MaPhienBan, MaChiNhanh, SoLuong, TrangThaiTiepNhan, NguoiPhanHoi, NgayPhanHoi, LyDoTuChoi) VALUES
(1,  1,  31, 'CN01', 1, N'Đã tiếp nhận', 'quanly1', '2026-02-15 09:30:00', NULL),
(2,  2,  3,  'CN02', 1, N'Đã tiếp nhận', 'quanly2', '2026-03-01 14:00:00', NULL),
(3,  3,  59, 'CN03', 1, N'Đã tiếp nhận', 'quanly3', '2026-03-15 10:00:00', NULL),
(4,  4,  24, 'CN04', 1, N'Đã tiếp nhận', 'quanly4', '2026-04-05 15:30:00', NULL),
(5,  5,  41, 'CN05', 1, N'Đã tiếp nhận', 'quanly5', '2026-04-10 08:00:00', NULL),
(6,  6,  37, 'CN06', 1, N'Đã tiếp nhận', 'quanly6', '2026-04-25 11:00:00', NULL),
(7,  7,  2,  'CN01', 1, N'Đã tiếp nhận', 'quanly1', '2026-05-10 09:00:00', NULL),
(8,  8,  11, 'CN02', 1, N'Đã tiếp nhận', 'quanly2', '2026-05-25 11:30:00', NULL),
(9,  9,  75, 'CN03', 1, N'Đã tiếp nhận', 'quanly3', '2026-07-05 14:00:00', NULL),
(10, 10, 17, 'CN01', 1, N'Chờ xác nhận', NULL, NULL, NULL),
(11, 11, 46, 'CN02', 1, N'Chờ xác nhận', NULL, NULL, NULL),
(12, 12, 67, 'CN03', 1, N'Chờ xác nhận', NULL, NULL, NULL),
(13, 13, 33, 'CN04', 1, N'Từ chối', 'quanly4', '2026-05-01 16:00:00', N'Khách đổi ý không mua nữa'),
(14, 14, 9,  'CN05', 1, N'Từ chối', 'quanly5', '2026-05-20 10:30:00', N'Không đủ khả năng tài chính'),
(15, 15, 61, 'CN06', 1, N'Từ chối', 'quanly6', '2026-07-05 15:00:00', N'Chọn mua dòng xe khác');
SET IDENTITY_INSERT DonDatCocChiTiet OFF;

-- ==================== 11. THÊM HÓA ĐƠN MẪU ====================
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, MaChiNhanh, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000001', 1, 'user1', 31, 'quanly1', 'CN01', 1100000000, 110000000, 50000000, 1160000000, 1160000000, N'Chuyển khoản + Tiền mặt', '2026-02-20', 'WDB1111111A000001', 'M274000001', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, MaChiNhanh, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000002', 2, 'user2', 3,  'quanly2', 'CN02', 950000000, 95000000, 30000000, 1015000000, 1015000000, N'Chuyển khoản', '2026-03-05', 'WDB2222222A000002', 'M274000002', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, MaChiNhanh, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000003', 3, 'user3', 59, 'quanly3', 'CN03', 1150000000, 115000000, 80000000, 1170000000, 1170000000, N'Chuyển khoản', '2026-03-28', 'WDB3333333A000003', 'M274000003', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, MaChiNhanh, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000004', 4, 'user4', 24, 'quanly4', 'CN04', 1600000000, 160000000, 120000000, 1640000000, 1640000000, N'Chuyển khoản', '2026-04-15', 'WDB4444444A000004', 'M274000004', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, MaChiNhanh, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000005', 5, 'user5', 41, 'quanly5', 'CN05', 1500000000, 150000000, 100000000, 1550000000, 1550000000, N'Chuyển khoản', '2026-04-20', 'WDB5555555A000005', 'M274000005', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, MaChiNhanh, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000006', 6, 'user1', 37, 'quanly6', 'CN06', 1050000000, 105000000, 105000000, 1050000000, 1050000000, N'Chuyển khoản', '2026-05-10', 'WDB6666666A000006', 'M274000006', N'Đã thanh toán');

-- ==================== 12. THỐNG KÊ DOANH THU (TÍNH TỪ DỮ LIỆU THỰC TẾ) ====================
-- Tháng 1: Don 1 (CN01, 200M), chưa có hóa đơn
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN01', 0, 200000000, 0, 0, 15000, 45, 18);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN02', 0, 0, 0, 0, 12000, 30, 2);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN03', 0, 0, 0, 0, 9000, 25, 35);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN04', 0, 0, 0, 0, 8000, 20, 14);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN05', 0, 0, 0, 0, 5000, 12, 23);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN06', 0, 0, 0, 0, 4000, 8, 21);
-- Tháng 2: HD000001 (CN01, 1160M), Don 2 (CN02, 300M)
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN01', 1160000000, 0, 1, 0, 18000, 55, 18);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN02', 0, 300000000, 0, 0, 14000, 35, 2);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN03', 0, 0, 0, 0, 10000, 28, 35);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN04', 0, 0, 0, 0, 8500, 22, 14);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN06', 0, 0, 0, 0, 6000, 15, 21);
-- Tháng 3: HD000002 (CN02, 1015M), HD000003 (CN03, 1170M), Don 3 (CN03, 150M), Don 4 (CN04, 450M), Don 5 (CN05, 300M)
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN02', 1015000000, 0, 1, 0, 16000, 40, 2);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN03', 1170000000, 150000000, 1, 0, 12000, 32, 35);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN04', 0, 450000000, 0, 0, 11000, 30, 14);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN05', 0, 300000000, 0, 0, 7000, 18, 23);
-- Tháng 4: HD000004 (CN04, 1640M), HD000005 (CN05, 1550M), Don 7 (CN01, 250M), Don 6 (CN06, 250M), Don 13 hủy (CN04, 100M)
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN01', 0, 250000000, 0, 0, 20000, 60, 18);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN04', 1640000000, 0, 1, 1, 12000, 32, 14);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN05', 1550000000, 0, 1, 0, 9000, 24, 23);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN06', 0, 250000000, 0, 0, 7000, 16, 21);
-- Tháng 5: HD000006 (CN06, 1050M), Don 8 (CN02, 180M), Don 14 hủy (CN05, 150M)
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN02', 0, 180000000, 0, 0, 15000, 38, 2);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN05', 0, 0, 0, 1, 8000, 20, 23);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN06', 1050000000, 0, 1, 0, 8000, 20, 21);
-- Tháng 6: Don 9 (CN03, 100M), Don 10 (CN01, 200M), Don 11 (CN02, 350M), Don 15 hủy (CN06, 80M)
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN01', 0, 200000000, 0, 0, 18000, 50, 18);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN02', 0, 350000000, 0, 0, 16000, 42, 2);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN03', 0, 100000000, 0, 0, 10000, 26, 35);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN06', 0, 0, 0, 1, 7000, 18, 21);
-- Tháng 7: Don 12 (CN03, 150M)
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-07', 'CN03', 0, 150000000, 0, 0, 6000, 15, 35);

-- ==================== 13. CẬP NHẬT TỒN KHO ====================
-- Trừ tồn kho cho từng xe đã bán (hóa đơn đã thanh toán)
UPDATE PhienBanXe_SanPham SET SoLuongTrongKho = SoLuongTrongKho - 1 WHERE MaPhienBan = 31; -- Lux SA 2.0T (HD000001)
UPDATE PhienBanXe_SanPham SET SoLuongTrongKho = SoLuongTrongKho - 1 WHERE MaPhienBan = 3;  -- Hilux 2.4G (HD000002)
UPDATE PhienBanXe_SanPham SET SoLuongTrongKho = SoLuongTrongKho - 1 WHERE MaPhienBan = 59; -- Santa Fe 2.5 Premium (HD000003)
UPDATE PhienBanXe_SanPham SET SoLuongTrongKho = SoLuongTrongKho - 1 WHERE MaPhienBan = 24; -- 320i Sport Line (HD000004)
UPDATE PhienBanXe_SanPham SET SoLuongTrongKho = SoLuongTrongKho - 1 WHERE MaPhienBan = 41; -- C200 Avantgarde (HD000005)
UPDATE PhienBanXe_SanPham SET SoLuongTrongKho = SoLuongTrongKho - 1 WHERE MaPhienBan = 37; -- VF 8 Eco (HD000006)

-- ==================== 14. CẬP NHẬT MẬT KHẨU (PBKDF2 hash) ====================
UPDATE TaiKhoan SET MatKhau = N'vmkvfKErSh20Mgk5RAFnXA==.cpvftbad6YQAJyb4tnvFXlt0cVJnLzEufbEDLpv4XvA=' WHERE TenDangNhap = N'fntzzs682@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'Y6c07iVOADL1QREtk9ICoA==.uYBbMN2BI+5id0mITtfxzQ7qJR487SxvnjhfKpQCWTA=' WHERE TenDangNhap = N'minhquanmkp123@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'VXiDw90yeiJX4xbvy8InFw==.AhO+0sz6u9keAaY4KqoeFCfMmoo9eN/xG7xPBGcpOv4=' WHERE TenDangNhap = N'Ngttu2006@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'4DU4sEyLcRCLWX0vIpXxJg==.+fXasslu3CZmZtAJi+U110CPQKU2dbUBB4OUKxHZj2g=' WHERE TenDangNhap = N'QuanlyCS1';
UPDATE TaiKhoan SET MatKhau = N'ThwDARdiyQS7W/7MMrbmlQ==.WpTtXOgkUGLiWJkxYgbZFDBM4nAInhVzxIDJPVHb93s=' WHERE TenDangNhap = N'QuanlyCS2';
UPDATE TaiKhoan SET MatKhau = N'5qT9B1D94Yz/9audhLhPxA==.piwvwCnjJtqj3Obai31WLUnsQaAWHdSYHOD+THOzexE=' WHERE TenDangNhap = N'QuanlyCS3';
UPDATE TaiKhoan SET MatKhau = N'smf2YkvpetqaLwWd4dcprw==.GnP0TPr4mSCkfX1vD5bCit7tn8ejljC7IzE/XGrgX0A=' WHERE TenDangNhap = N'QuanlyCS4';
UPDATE TaiKhoan SET MatKhau = N'nOhqvRmr8cBnHyIummlhng==.AFbaAqdHTugwhsTsmyR2eImD394d/94+VbRP5dpOTQo=' WHERE TenDangNhap = N'QuanlyCS5';
UPDATE TaiKhoan SET MatKhau = N'plVucQYEuBjLD7tICOBPsA==.YQQ77Aw2dPIADfyV88SXO0/9fEIDy9+ozZbfHY+NYiM=' WHERE TenDangNhap = N'QuanlyCS6';
UPDATE TaiKhoan SET MatKhau = N'mONfji6a2TCRf2SkxVC4XA==.m2xfTGs9IxY/zA48jYvMeQyvQLNV3ojEPsCVFR/9G2g=' WHERE TenDangNhap = N'thanhdac223@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'RrGdx0932vTBbe7TtC7XMg==.z1yGHr6z9T7jW3le5pODP7+8qwwKY8fvoMh/63D/Qgg=' WHERE TenDangNhap = N'user1';
UPDATE TaiKhoan SET MatKhau = N'D+gvKgilUrYNxWSGjLBiiA==.FejuI2thkV/i8d+2CnK2Kf9o2eJ+KaZqfHNU88L/Rao=' WHERE TenDangNhap = N'user2';
UPDATE TaiKhoan SET MatKhau = N'FGAO0dhh4hpX6IXg7UP7Xg==.VgoHnnaKcU+96KaRIw/fEIzJvxXktpfS3KwrVPg9y2E=' WHERE TenDangNhap = N'user3';
UPDATE TaiKhoan SET MatKhau = N'pqJk4eU0McL3ZIbXAs5Htw==.n2zLZfvFFjRXMpCmA4DN2kYmITsC1HhPZk8oiH6zi/k=' WHERE TenDangNhap = N'user4';
UPDATE TaiKhoan SET MatKhau = N'JkczGiUWrJPGO1KiE8kwBw==.uHc6YGr9tVyXdVOQlt47dXLyo+7vzOMZCZRAGv5MgMc=' WHERE TenDangNhap = N'user5';
UPDATE TaiKhoan SET MatKhau = N'cG2v6iFxx5mDaZKKjeQ2vw==.ExgqY3iSU1Nl4zPNCgItChA1pWJX/XzHfOC/dKFXNGw=' WHERE TenDangNhap = N'Vanh280306@gmail.com';

-- ==================== 15. XEM LẠI DỮ LIỆU ====================
PRINT N'=== HÃNG XE ==='; SELECT * FROM HangXe ORDER BY MaHang;
PRINT N'=== DÒNG XE ==='; SELECT d.MaDong, d.TenDong, h.TenHang, d.KieuDang FROM DongXe d JOIN HangXe h ON d.MaHang = h.MaHang ORDER BY d.MaDong;
PRINT N'=== PHIÊN BẢN ==='; SELECT p.MaPhienBan, p.TenPhienBan, d.TenDong, h.TenHang, p.GiaNiemYet, p.SoLuongTrongKho, p.TrangThai FROM PhienBanXe_SanPham p JOIN DongXe d ON p.MaDong = d.MaDong JOIN HangXe h ON d.MaHang = h.MaHang ORDER BY p.MaPhienBan;
PRINT N'=== TÀI KHOẢN ==='; SELECT MaTaiKhoan AS ID, TenDangNhap, TenHienThi, Email, VaiTro, TrangThai FROM TaiKhoan ORDER BY MaTaiKhoan;
PRINT N'=== CHI NHÁNH ==='; SELECT * FROM ChiNhanhShowroom;
PRINT N'=== KHUYẾN MÃI ==='; SELECT * FROM ChuongTrinhKhuyenMai ORDER BY MaKhuyenMai;
PRINT N'=== ĐƠN CỌC ==='; SELECT * FROM DonDatCoc ORDER BY NgayTaoDon DESC;
PRINT N'=== CHI TIẾT ĐƠN CỌC ==='; SELECT ct.*, d.TenPhienBan FROM DonDatCocChiTiet ct JOIN PhienBanXe_SanPham d ON ct.MaPhienBan = d.MaPhienBan ORDER BY ct.MaChiTiet;
PRINT N'=== TỒN KHO THEO CHI NHÁNH ==='; SELECT t.MaTonKho, t.MaPhienBan, d.TenPhienBan, t.MaChiNhanh, t.SoLuong, t.NgayCapNhat FROM TonKhoTheoChiNhanh t JOIN PhienBanXe_SanPham d ON t.MaPhienBan = d.MaPhienBan ORDER BY t.MaPhienBan, t.MaChiNhanh;
PRINT N'=== HÓA ĐƠN ==='; SELECT * FROM HoaDonMuaXe ORDER BY NgayXuatHoaDon DESC;
PRINT N'=== THỐNG KÊ ==='; SELECT * FROM ThongKeTongHop_Boss ORDER BY KyBaoCao;

-- Đếm số lượng
PRINT N'=== SỐ LƯỢNG ===';
SELECT 'HangXe' AS Bang, COUNT(*) AS SoLuong FROM HangXe
UNION ALL SELECT 'DongXe', COUNT(*) FROM DongXe
UNION ALL SELECT 'PhienBanXe_SanPham', COUNT(*) FROM PhienBanXe_SanPham
UNION ALL SELECT 'TaiKhoan', COUNT(*) FROM TaiKhoan
UNION ALL SELECT 'DonDatCoc', COUNT(*) FROM DonDatCoc
UNION ALL SELECT 'DonDatCocChiTiet', COUNT(*) FROM DonDatCocChiTiet
UNION ALL SELECT 'TonKhoTheoChiNhanh', COUNT(*) FROM TonKhoTheoChiNhanh
UNION ALL SELECT 'HoaDonMuaXe', COUNT(*) FROM HoaDonMuaXe
UNION ALL SELECT 'ChiNhanhShowroom', COUNT(*) FROM ChiNhanhShowroom
UNION ALL SELECT 'ThongKeTongHop_Boss', COUNT(*) FROM ThongKeTongHop_Boss
ORDER BY Bang;


-- ============================================
-- PHẦN 3: VÍ DỤ CẬP NHẬT TRẠNG THÁI (CHẠY RIÊNG)
-- ============================================
-- Chạy 3 lệnh UPDATE này để test Quản Lý flow nhanh:

-- 1. Đặt 3 lịch hẹn thành "Chờ xác nhận" để QL có thể duyệt/từ chối
UPDATE LichHenLaiThu SET TrangThai = N'Chờ xác nhận' WHERE TrangThai IS NULL OR TrangThai = N'';
UPDATE LichHenLaiThu SET MaChiNhanh = 'MB001' WHERE MaChiNhanh IS NULL;

-- 2. Đặt 3 đơn cọc thành "Chờ xác nhận" để QL có thể duyệt/hủy
UPDATE DonDatCoc SET TrangThaiDonHang = N'Chờ xác nhận' WHERE TrangThaiDonHang IS NULL OR TrangThaiDonHang = N'';
UPDATE DonDatCoc SET MaChiNhanh = 'MB001' WHERE MaChiNhanh IS NULL;

-- 3. Kiểm tra kết quả
SELECT MaLichHen, MaChiNhanh, TrangThai FROM LichHenLaiThu ORDER BY MaLichHen;
SELECT MaDonCoc, MaChiNhanh, TrangThaiDonHang FROM DonDatCoc ORDER BY MaDonCoc;
