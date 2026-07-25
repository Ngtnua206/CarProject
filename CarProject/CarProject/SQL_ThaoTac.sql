-- ===================================================
-- SQL thao tác nhanh cho CarProject
-- Chạy trong SSMS (chọn database CarShopDb trước)
-- Thứ tự từ trên xuống dưới, chạy phát ok luôn
-- ===================================================

-- ==================== 1. XÓA DỮ LIỆU CŨ ====================
DELETE FROM ThongKeTongHop_Boss;
DELETE FROM HoaDonMuaXe;
DELETE FROM DonDatCoc;
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
VALUES ('fntzzs682@gmail.com', 'Iumaioanhh@2024', 'Admin', 'Active', 'Admin', 'fntzzs682@gmail.com');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('Ngttu2006@gmail.com', 'Iumaioanhh@2024', 'Admin', 'Active', N'Ngtnua', 'Ngttu2006@gmail.com');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('thanhdac223@gmail.com', 'Iumaioanhh@2024', 'Admin', 'Active', '', 'thanhdac223@gmail.com');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi, Email)
VALUES ('Vanh280306@gmail.com', 'Iumaioanhh@2024', 'Admin', 'Active', '', 'Vanh280306@gmail.com');
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
VALUES ('quanly1', 'quanly123', N'Quản Lý', N'Hoạt động', N'Nguyễn Văn A');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('quanly2', 'quanly123', N'Quản Lý', N'Hoạt động', N'Nguyễn Văn B');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('quanly3', 'quanly123', N'Quản Lý', N'Hoạt động', N'Trần Thị C');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('quanly4', 'quanly123', N'Quản Lý', N'Hoạt động', N'Lê Văn D');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('quanly5', 'quanly123', N'Quản Lý', N'Hoạt động', N'Phạm Thị E');
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
VALUES ('quanly6', 'quanly123', N'Quản Lý', N'Hoạt động', N'Hoàng Văn F');

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
INSERT INTO DongXe (MaDong, MaHang, TenDong, KieuDang) VALUES
-- Toyota (1)
(1,  1,  N'Toyota Camry',          N'Sedan'),
(2,  1,  N'Toyota Hilux',          N'Bán tải'),
(3,  1,  N'Toyota Corolla Altis',  N'Sedan'),
(4,  1,  N'Toyota Fortuner',       N'SUV'),
(5,  1,  N'Toyota Vios',           N'Sedan'),
-- Honda (2)
(6,  2,  N'Honda Civic',           N'Sedan'),
(7,  2,  N'Honda CR-V',            N'SUV'),
(8,  2,  N'Honda HR-V',            N'SUV'),
(9,  2,  N'Honda Accord',          N'Sedan'),
-- Ford (3)
(10, 3,  N'Ford Explorer',         N'SUV'),
(11, 3,  N'Ford Everest',          N'SUV'),
(12, 3,  N'Ford Ranger',           N'Bán tải'),
(13, 3,  N'Ford Territory',        N'SUV'),
-- BMW (4)
(14, 4,  N'BMW 3 Series',          N'Sedan'),
(15, 4,  N'BMW 5 Series',          N'Sedan'),
(16, 4,  N'BMW X3',                N'SUV'),
(17, 4,  N'BMW X5',                N'SUV'),
-- VinFast (5)
(18, 5,  N'VinFast Lux SA',        N'SUV'),
(19, 5,  N'VinFast Fadil',         N'Hatchback'),
(20, 5,  N'VinFast VF e34',        N'SUV (Điện)'),
(21, 5,  N'VinFast VF 8',          N'SUV (Điện)'),
(22, 5,  N'VinFast VF 9',          N'SUV (Điện)'),
-- Mercedes-Benz (6)
(23, 6,  N'Mercedes C-Class',      N'Sedan'),
(24, 6,  N'Mercedes E-Class',      N'Sedan'),
(25, 6,  N'Mercedes S-Class',      N'Sedan'),
(26, 6,  N'Mercedes GLC',          N'SUV'),
(27, 6,  N'Mercedes GLE',          N'SUV'),
-- Audi (7)
(28, 7,  N'Audi A3',               N'Sedan'),
(29, 7,  N'Audi A4',               N'Sedan'),
(30, 7,  N'Audi Q5',               N'SUV'),
(31, 7,  N'Audi Q7',               N'SUV'),
-- Lexus (8)
(32, 8,  N'Lexus ES',              N'Sedan'),
(33, 8,  N'Lexus RX',              N'SUV'),
(34, 8,  N'Lexus NX',              N'SUV'),
-- Hyundai (9)
(35, 9,  N'Hyundai Santa Fe',      N'SUV'),
(36, 9,  N'Hyundai Tucson',        N'SUV'),
(37, 9,  N'Hyundai Accent',        N'Sedan'),
(38, 9,  N'Hyundai Creta',         N'SUV'),
-- Kia (10)
(39, 10, N'Kia Sorento',           N'SUV'),
(40, 10, N'Kia Sportage',          N'SUV'),
(41, 10, N'Kia Cerato',            N'Sedan'),
(42, 10, N'Kia Morning',           N'Hatchback'),
-- Mazda (11)
(43, 11, N'Mazda CX-5',            N'SUV'),
(44, 11, N'Mazda CX-8',            N'SUV'),
(45, 11, N'Mazda3',                N'Sedan'),
(46, 11, N'Mazda6',                N'Sedan'),
-- Suzuki (12)
(47, 12, N'Suzuki Swift',          N'Hatchback'),
(48, 12, N'Suzuki Vitara',         N'SUV'),
(49, 12, N'Suzuki Ertiga',         N'MPV'),
-- Mitsubishi (13)
(50, 13, N'Mitsubishi Xpander',    N'MPV'),
(51, 13, N'Mitsubishi Outlander',  N'SUV'),
(52, 13, N'Mitsubishi Triton',     N'Bán tải'),
-- Nissan (14)
(53, 14, N'Nissan Navara',         N'Bán tải'),
(54, 14, N'Nissan Kicks',          N'SUV'),
(55, 14, N'Nissan Almera',         N'Sedan'),
-- Subaru (15)
(56, 15, N'Subaru Forester',       N'SUV'),
(57, 15, N'Subaru Outback',        N'SUV'),
(58, 15, N'Subaru XV',             N'SUV');
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

-- ==================== 7. THÊM KHUYẾN MÃI ====================
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM01', N'Giảm 50% lệ phí trước bạ', N'Áp dụng cho tất cả dòng xe', N'Phần trăm', 50, 50000000, '2024-01-01', '2024-03-31', N'Hoạt động');
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM02', N'Giảm ngay 20 triệu', N'Cho dòng xe Toyota Vios và Hyundai Accent', N'Số tiền', 20000000, NULL, '2026-07-01', '2026-09-30', N'Hoạt động');
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM03', N'Tặng gói phụ kiện 15 triệu', N'Cho khách đặt cọc xe Mercedes trước 30/09', N'Số tiền', 15000000, NULL, '2026-07-01', '2026-09-30', N'Hoạt động');
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
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (1, 'user1', 31, 'quanly1', 200000000, N'Chuyển khoản', N'Đã thanh toán', '2026-01-15 09:30:00', '2026-02-20', N'Đã giao xe', N'Giao tại showroom Quận 7', N'Nguyễn Văn A', '0901000001', N'123 Lê Lợi, Quận 1', 'MGC250701-1');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (2, 'user2', 3,  'quanly2', 300000000, N'Chuyển khoản', N'Đã thanh toán', '2026-02-10 14:00:00', '2026-03-05', N'Đã giao xe', N'Xe màu bạc', N'Trần Văn B', '0901000002', N'456 Nguyễn Huệ, Quận 1', 'MGC250702-2');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (3, 'user3', 59, 'quanly3', 150000000, N'Tiền mặt', N'Đã thanh toán', '2026-03-05 10:00:00', '2026-03-28', N'Đã giao xe', N'Giao tại showroom Cầu Giấy', N'Lê Thị C', '0901000003', N'789 Trần Hưng Đạo, Hoàn Kiếm', 'MGC250703-3');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (4, 'user4', 24, 'quanly4', 450000000, N'Chuyển khoản', N'Đã thanh toán', '2026-03-20 15:30:00', '2026-04-15', N'Đã giao xe', N'Khách VIP - gói phụ kiện', N'Phạm Văn D', '0901000004', N'123 Nguyễn Văn Linh, Quận 7', 'MGC250704-4');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (5, 'user5', 41, 'quanly5', 300000000, N'Chuyển khoản', N'Đã thanh toán', '2026-03-25 08:00:00', '2026-04-20', N'Đã giao xe', N'Mercedes C200 màu bạc', N'Đỗ Thúy Hằng', '0901000011', N'456 Nguyễn Văn Linh, Đà Nẵng', 'MGC250711-11');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (6, 'user1', 37, 'quanly6', 250000000, N'Chuyển khoản', N'Đã thanh toán', '2026-04-05 11:00:00', '2026-05-10', N'Đã giao xe', N'VF 8 màu xanh', N'Nguyễn Văn G', '0901000012', N'789 Văn Cao, Hải Phòng', 'MGC250712-12');
-- Đã xác nhận (MaDonCoc 7-9)
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (7, 'user2', 2,  'quanly1', 250000000, N'Chuyển khoản', N'Đã thanh toán', '2026-04-10 09:00:00', '2026-05-20', N'Đã xác nhận', N'Camry Hybrid màu đen', N'Hoàng Thị E', '0901000005', N'456 Hải Phòng, Đà Nẵng', 'MGC250705-5');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (8, 'user3', 11, 'quanly2', 180000000, N'Tiền mặt', N'Đã thanh toán', '2026-05-05 11:30:00', '2026-06-10', N'Đã xác nhận', N'Civic Turbo màu đỏ', N'Đặng Văn F', '0901000006', N'789 Văn Cao, Hải Phòng', 'MGC250706-6');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (9, 'user4', 75, 'quanly3', 100000000, N'Chuyển khoản', N'Đã thanh toán', '2026-06-20 14:00:00', '2026-07-25', N'Đã xác nhận', N'CX-5 Deluxe màu đỏ', N'Vũ Thị M', '0901000013', N'123 Hoàng Quốc Việt, Cầu Giấy', 'MGC250713-13');
-- Chờ xác nhận (MaDonCoc 10-12)
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (10, 'user5', 17, 'quanly1', 200000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-06-01 08:00:00', N'Chờ xác nhận', N'Khách đang chờ vay ngân hàng', N'Nguyễn Văn H', '0901000007', N'123 Quận 7, TP.HCM', 'MGC250707-7');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (11, 'user1', 46, 'quanly2', 350000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-06-15 14:00:00', N'Chờ xác nhận', N'Khách muốn lái thử trước', N'Trần Thị K', '0901000008', N'456 Thủ Đức, TP.HCM', 'MGC250708-8');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (12, 'user2', 67, 'quanly3', 150000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-07-01 09:00:00', N'Chờ xác nhận', N'Đang thương lượng giá', N'Lê Văn P', '0901000014', N'789 Cầu Giấy, Hà Nội', 'MGC250714-14');
-- Đã hủy (MaDonCoc 13-15)
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (13, 'user3', 33, 'quanly4', 100000000, N'Tiền mặt', N'Đã hoàn tiền', '2026-04-20 16:00:00', N'Đã hủy', N'Khách đổi ý không mua nữa', N'Lê Văn I', '0901000009', N'789 Cầu Giấy, Hà Nội', 'MGC250709-9');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (14, 'user4', 9,  'quanly5', 150000000, N'Chuyển khoản', N'Đã hoàn tiền', '2026-05-10 10:30:00', N'Đã hủy', N'Không đủ khả năng tài chính', N'Phạm Thị K', '0901000010', N'321 Hoàng Mai, Hà Nội', 'MGC250710-10');
INSERT INTO DonDatCoc (MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
VALUES (15, 'user5', 61, 'quanly6', 80000000, N'Chuyển khoản', N'Đã hoàn tiền', '2026-06-25 15:00:00', N'Đã hủy', N'Chọn mua dòng xe khác', N'Ngô Văn Q', '0901000015', N'456 Lê Lợi, Hải Phòng', 'MGC250715-15');
SET IDENTITY_INSERT DonDatCoc OFF;

-- ==================== 11. THÊM HÓA ĐƠN MẪU ====================
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000001', 1, 'user1', 31, 'quanly1', 1100000000, 110000000, 50000000, 1160000000, 1160000000, N'Chuyển khoản + Tiền mặt', '2026-02-20', 'WDB1111111A000001', 'M274000001', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000002', 2, 'user2', 3,  'quanly2', 950000000, 95000000, 30000000, 1015000000, 1015000000, N'Chuyển khoản', '2026-03-05', 'WDB2222222A000002', 'M274000002', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000003', 3, 'user3', 59, 'quanly3', 1150000000, 115000000, 80000000, 1170000000, 1170000000, N'Chuyển khoản', '2026-03-28', 'WDB3333333A000003', 'M274000003', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000004', 4, 'user4', 24, 'quanly4', 1600000000, 160000000, 120000000, 1640000000, 1640000000, N'Chuyển khoản', '2026-04-15', 'WDB4444444A000004', 'M274000004', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000005', 5, 'user5', 41, 'quanly5', 1500000000, 150000000, 100000000, 1550000000, 1550000000, N'Chuyển khoản', '2026-04-20', 'WDB5555555A000005', 'M274000005', N'Đã thanh toán');
INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
VALUES ('HD000006', 6, 'user1', 37, 'quanly6', 1050000000, 105000000, 105000000, 1050000000, 1050000000, N'Chuyển khoản', '2026-05-10', 'WDB6666666A000006', 'M274000006', N'Đã thanh toán');

-- ==================== 12. THÊM THỐNG KÊ DOANH THU ====================
-- Tháng 1
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN01', 1160000000, 200000000, 1, 0, 15000, 45, 31);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN02', 500000000, 80000000, 0, 1, 12000, 30, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN03', 850000000, 150000000, 0, 0, 9000, 25, 59);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN04', 300000000, 50000000, 0, 0, 8000, 20, 24);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN05', 200000000, 30000000, 0, 0, 5000, 12, 17);
-- Tháng 2
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN01', 2100000000, 300000000, 2, 0, 18000, 55, 31);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN02', 1015000000, 300000000, 1, 0, 14000, 35, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN03', 600000000, 100000000, 0, 0, 10000, 28, 59);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN06', 450000000, 80000000, 0, 0, 6000, 15, 9);
-- Tháng 3
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN01', 2800000000, 400000000, 2, 0, 22000, 65, 31);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN02', 1800000000, 250000000, 1, 1, 16000, 40, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN03', 1170000000, 150000000, 1, 0, 12000, 32, 59);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN04', 1640000000, 450000000, 1, 0, 11000, 30, 24);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN05', 800000000, 250000000, 0, 0, 7000, 18, 17);
-- Tháng 4
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN01', 2800000000, 400000000, 2, 0, 20000, 60, 31);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN02', 1400000000, 200000000, 1, 0, 15000, 38, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN03', 1500000000, 200000000, 1, 0, 13000, 35, 59);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN04', 1750000000, 300000000, 1, 0, 12000, 32, 24);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN05', 1550000000, 300000000, 1, 0, 9000, 24, 41);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN06', 700000000, 100000000, 0, 0, 7000, 16, 9);
-- Tháng 5
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN01', 3200000000, 500000000, 3, 1, 24000, 70, 31);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN02', 2000000000, 350000000, 1, 0, 18000, 45, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN03', 1600000000, 200000000, 1, 1, 14000, 35, 59);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN04', 1900000000, 350000000, 1, 0, 14000, 36, 24);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN05', 1050000000, 250000000, 1, 0, 10000, 28, 37);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN06', 900000000, 180000000, 0, 0, 8000, 20, 9);
-- Tháng 6
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN01', 3800000000, 600000000, 3, 0, 26000, 75, 31);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN02', 2200000000, 350000000, 2, 0, 20000, 50, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN03', 1800000000, 250000000, 1, 0, 16000, 40, 59);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN04', 2100000000, 350000000, 1, 0, 14000, 35, 24);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN05', 1500000000, 280000000, 1, 0, 10000, 28, 37);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN06', 1000000000, 200000000, 0, 0, 9000, 22, 9);
-- Tháng 7
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-07', 'CN01', 1500000000, 200000000, 1, 0, 12000, 35, 31);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-07', 'CN02', 800000000, 100000000, 0, 0, 8000, 20, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-07', 'CN03', 600000000, 80000000, 0, 0, 6000, 15, 59);

-- ==================== 13. CẬP NHẬT MẬT KHẨU (PBKDF2 hash) ====================
UPDATE TaiKhoan SET MatKhau = N'wNqykWOAM5hGb8+mPUYvaw==.JpcQKcXxRJl31glDE1nu6/PUQsZAxLvS5j1YHAwxLT4=' WHERE TenDangNhap = N'fntzzs682@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'0OUCfE9U3HuDS2VghvJd3g==.FNV/tRowCFfYXFBgTlPgmDYep6OCLtZyQEArjrxE4Vg=' WHERE TenDangNhap = N'minhquanmkp123@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'jfj5pQ4ZwoZuOVn4pZCu1Q==.aDRo1vo42rnvA2i/E+B4kqvHddiwNog1Ztj/3tY7S1k=' WHERE TenDangNhap = N'Ngttu2006@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly1';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly2';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly3';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly4';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly5';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly6';
UPDATE TaiKhoan SET MatKhau = N'W/91Lmc0TaaA7wHTc+7Vvg==.usPMQz72I2LW9aN2XIPpxGP57LfvypZ+MPQPXWrxLmI=' WHERE TenDangNhap = N'thanhdac223@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'n2zN5/JWq7EppQ1RgTIZ4g==.HeZXQln7bNEKqIg6tC+U5w2z6N2SZiQ+wJI41bpdvYg=' WHERE TenDangNhap = N'user1';
UPDATE TaiKhoan SET MatKhau = N'n2zN5/JWq7EppQ1RgTIZ4g==.HeZXQln7bNEKqIg6tC+U5w2z6N2SZiQ+wJI41bpdvYg=' WHERE TenDangNhap = N'user2';
UPDATE TaiKhoan SET MatKhau = N'n2zN5/JWq7EppQ1RgTIZ4g==.HeZXQln7bNEKqIg6tC+U5w2z6N2SZiQ+wJI41bpdvYg=' WHERE TenDangNhap = N'user3';
UPDATE TaiKhoan SET MatKhau = N'n2zN5/JWq7EppQ1RgTIZ4g==.HeZXQln7bNEKqIg6tC+U5w2z6N2SZiQ+wJI41bpdvYg=' WHERE TenDangNhap = N'user4';
UPDATE TaiKhoan SET MatKhau = N'n2zN5/JWq7EppQ1RgTIZ4g==.HeZXQln7bNEKqIg6tC+U5w2z6N2SZiQ+wJI41bpdvYg=' WHERE TenDangNhap = N'user5';
UPDATE TaiKhoan SET MatKhau = N'NErALrnWhGZIsGdyZMHueg==.qC0xjUR/NjlOtz8ZdBImkgq8qxDt9elxyC7hIyF2p/E=' WHERE TenDangNhap = N'Vanh280306@gmail.com';

-- ==================== 14. XEM LẠI DỮ LIỆU ====================
PRINT N'=== HÃNG XE ==='; SELECT * FROM HangXe ORDER BY MaHang;
PRINT N'=== DÒNG XE ==='; SELECT d.MaDong, d.TenDong, h.TenHang, d.KieuDang FROM DongXe d JOIN HangXe h ON d.MaHang = h.MaHang ORDER BY d.MaDong;
PRINT N'=== PHIÊN BẢN ==='; SELECT p.MaPhienBan, p.TenPhienBan, d.TenDong, h.TenHang, p.GiaNiemYet, p.SoLuongTrongKho, p.TrangThai FROM PhienBanXe_SanPham p JOIN DongXe d ON p.MaDong = d.MaDong JOIN HangXe h ON d.MaHang = h.MaHang ORDER BY p.MaPhienBan;
PRINT N'=== TÀI KHOẢN ==='; SELECT MaTaiKhoan AS ID, TenDangNhap, TenHienThi, Email, VaiTro, TrangThai FROM TaiKhoan ORDER BY MaTaiKhoan;
PRINT N'=== CHI NHÁNH ==='; SELECT * FROM ChiNhanhShowroom;
PRINT N'=== KHUYẾN MÃI ==='; SELECT * FROM ChuongTrinhKhuyenMai ORDER BY MaKhuyenMai;
PRINT N'=== ĐƠN CỌC ==='; SELECT * FROM DonDatCoc ORDER BY NgayTaoDon DESC;
PRINT N'=== HÓA ĐƠN ==='; SELECT * FROM HoaDonMuaXe ORDER BY NgayXuatHoaDon DESC;
PRINT N'=== THỐNG KÊ ==='; SELECT * FROM ThongKeTongHop_Boss ORDER BY KyBaoCao;

-- Đếm số lượng
PRINT N'=== SỐ LƯỢNG ===';
SELECT 'HangXe' AS Bang, COUNT(*) AS SoLuong FROM HangXe
UNION ALL SELECT 'DongXe', COUNT(*) FROM DongXe
UNION ALL SELECT 'PhienBanXe_SanPham', COUNT(*) FROM PhienBanXe_SanPham
UNION ALL SELECT 'TaiKhoan', COUNT(*) FROM TaiKhoan
UNION ALL SELECT 'DonDatCoc', COUNT(*) FROM DonDatCoc
UNION ALL SELECT 'HoaDonMuaXe', COUNT(*) FROM HoaDonMuaXe
UNION ALL SELECT 'ChiNhanhShowroom', COUNT(*) FROM ChiNhanhShowroom
UNION ALL SELECT 'ThongKeTongHop_Boss', COUNT(*) FROM ThongKeTongHop_Boss
ORDER BY Bang;
