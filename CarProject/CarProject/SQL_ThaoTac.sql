-- ===================================================
-- SQL thao tác nhanh cho CarProject
-- Chạy trong SSMS (chọn database CarShopDb trước)
-- Chỉnh sửa giá trị trực tiếp rồi chạy
-- ===================================================

-- ==================== XEM DỮ LIỆU ====================

-- Hãng xe
SELECT * FROM HangXe ORDER BY MaHang;

-- Dòng xe kèm tên hãng
SELECT d.MaDong, d.TenDong, h.TenHang, d.KieuDang
FROM DongXe d JOIN HangXe h ON d.MaHang = h.MaHang
ORDER BY d.MaDong;

-- Phiên bản xe kèm dòng xe + hãng
SELECT p.MaPhienBan, p.TenPhienBan, d.TenDong, h.TenHang,
       p.GiaNiemYet, p.SoLuongTrongKho, p.TrangThai
FROM PhienBanXe_SanPham p
JOIN DongXe d ON p.MaDong = d.MaDong
JOIN HangXe h ON d.MaHang = h.MaHang
ORDER BY p.MaPhienBan;

-- Tài khoản (TenDangNhap là khóa chính)
SELECT TenDangNhap, VaiTro, TrangThai FROM TaiKhoan ORDER BY TenDangNhap;
select * from TaiKhoan

-- Chi nhánh
SELECT * FROM ChiNhanhShowroom;

-- Khuyến mãi
SELECT * FROM ChuongTrinhKhuyenMai ORDER BY MaKhuyenMai;

-- Đơn cọc
SELECT * FROM DonDatCoc ORDER BY NgayTaoDon DESC;

-- Hóa đơn
SELECT * FROM HoaDonMuaXe ORDER BY NgayXuatHoaDon DESC;

-- Lịch hẹn lái thử
SELECT * FROM LichHenLaiThu ORDER BY NgayHen DESC;

-- Banner
SELECT * FROM QuangCaoBanner ORDER BY ThuTuHienThi;

-- Kênh tư vấn
SELECT * FROM KenhTuVan;

-- Thống kê
SELECT * FROM ThongKeTongHop_Boss ORDER BY KyBaoCao;

-- Nhật ký hoạt động
SELECT * FROM NhatKyHeThong ORDER BY ThoiGian DESC;


-- ==================== THÊM DỮ LIỆU ====================

-- Thêm tài khoản
select * from Taikhoan;


INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai)
VALUES ('minhquanmkp123@gmail.com', 'hahahihi123', 'Admin', 'Active');

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai)
VALUES ('user1', 'user123', 'User', 'Active');

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai)
VALUES ('quanly1', 'quanly123', N'Quản Lý', N'Hoạt động');

-- Thêm hãng xe (MaHang tự tăng)
INSERT INTO HangXe (TenHang, QuocGia, DuongDanLogo)
VALUES ('Mercedes-Benz', N'Đức', '/images/brands/mercedes.png');

-- Thêm dòng xe (MaHang lấy từ bảng HangXe)
INSERT INTO DongXe (MaHang, TenDong, KieuDang)
VALUES (1, 'Mercedes C-Class', 'Sedan');

-- Thêm phiên bản xe (MaDong lấy từ bảng DongXe)
INSERT INTO PhienBanXe_SanPham (MaDong, TenPhienBan, GiaNiemYet, MauSac, DongCo, HopSo, LoaiNhietLieu, SoLuongTrongKho, DuongDanAnh, TrangThai)
VALUES (1, 'C200 AMG', 1500000000, N'Đen', '2.0L Turbo', '9G-Tronic', N'Xăng', 5, '/images/cars/c200.jpg', N'Còn hàng');

-- Thêm chi nhánh (MaQuanLy = TenDangNhap của tài khoản quản lý)
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN01', N'Showroom Sài Gòn', N'123 Nguyễn Văn Linh Quận 7', N'TP.HCM', '0909123456', 'quanly1', N'Hoạt động');

-- Thêm khuyến mãi
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM01', N'Giảm 50% lệ phí trước bạ', N'Áp dụng cho tất cả dòng xe', N'Phần trăm', 50, 50000000, '2024-01-01', '2024-03-31', N'Hoạt động');

-- Thêm banner (MaQuanLyCapNhat = TenDangNhap của admin)
INSERT INTO QuangCaoBanner (DuongDanAnh, DuongDanLienKet, ThuTuHienThi, MaQuanLyCapNhat, TrangThaiKichHoat)
VALUES ('/images/banners/banner1.jpg', '/Details/1', 1, 'admin', 1);

-- Thêm kênh tư vấn
INSERT INTO KenhTuVan (UrlMessenger, UrlZalo, UrlSMS)
VALUES ('https://m.me/carshop', 'https://zalo.me/carshop', '0906123456');


-- ==================== SỬA DỮ LIỆU ====================

-- Sửa tên hãng
UPDATE HangXe SET TenHang = 'Mercedes-Benz Vietnam' WHERE MaHang = 1;

-- Sửa giá phiên bản
UPDATE PhienBanXe_SanPham SET GiaNiemYet = 1600000000 WHERE MaPhienBan = 1;

-- Sửa số lượng kho
UPDATE PhienBanXe_SanPham SET SoLuongTrongKho = 10 WHERE MaPhienBan = 1;

-- Sửa trạng thái phiên bản
UPDATE PhienBanXe_SanPham SET TrangThai = N'Hết hàng' WHERE MaPhienBan = 1;

-- Đổi quyền tài khoản
UPDATE TaiKhoan SET VaiTro = N'Quản Lý' WHERE TenDangNhap = 'user1';


UPDATE TaiKhoan SET AvatarUrl = null WHERE TenDangNhap = 'fntzzs682@gmail.com';

-- Kích hoạt/vô hiệu banner
UPDATE QuangCaoBanner SET TrangThaiKichHoat = 0 WHERE MaBanner = 1;


-- ==================== XÓA DỮ LIỆU ====================
-- Xóa theo thứ tự: con trước, cha sau

-- Xóa phiên bản xe
DELETE FROM PhienBanXe_SanPham WHERE MaPhienBan = 1;

-- Xóa dòng xe (phải xóa phiên bản trước)
DELETE FROM PhienBanXe_SanPham WHERE MaDong = 1;
DELETE FROM DongXe WHERE MaDong = 1;

-- Xóa hãng xe (phải xóa dòng xe + phiên bản trước)
DELETE FROM PhienBanXe_SanPham WHERE MaDong IN (SELECT MaDong FROM DongXe WHERE MaHang = 1);
DELETE FROM DongXe WHERE MaHang = 1;
DELETE FROM HangXe WHERE MaHang = 1;

-- Xóa tài khoản
DELETE FROM TaiKhoan WHERE TenDangNhap = 'quanly1';


-- ==================== TIỆN ÍCH ====================

-- Đếm số lượng từng bảng
SELECT 'HangXe' AS Bang, COUNT(*) AS SoLuong FROM HangXe
UNION ALL SELECT 'DongXe', COUNT(*) FROM DongXe
UNION ALL SELECT 'PhienBanXe_SanPham', COUNT(*) FROM PhienBanXe_SanPham
UNION ALL SELECT 'TaiKhoan', COUNT(*) FROM TaiKhoan
UNION ALL SELECT 'DonDatCoc', COUNT(*) FROM DonDatCoc
UNION ALL SELECT 'HoaDonMuaXe', COUNT(*) FROM HoaDonMuaXe
UNION ALL SELECT 'LichHenLaiThu', COUNT(*) FROM LichHenLaiThu
UNION ALL SELECT 'ChiNhanhShowroom', COUNT(*) FROM ChiNhanhShowroom
UNION ALL SELECT 'ChuongTrinhKhuyenMai', COUNT(*) FROM ChuongTrinhKhuyenMai
ORDER BY Bang;

-- Tìm xe theo tên
SELECT * FROM PhienBanXe_SanPham WHERE TenPhienBan LIKE '%C200%';

-- Tìm hóa đơn theo mã
SELECT * FROM HoaDonMuaXe WHERE MaHoaDon LIKE '%HD001%';


USE CarShopDb;
CREATE USER Vanh FOR LOGIN Vanh;
ALTER ROLE db_datareader ADD MEMBER Vanh;
ALTER ROLE db_datawriter ADD MEMBER Vanh;

-- ==================== CAP NHAT MAT KHAU (PBKDF2 hash) ====================
-- Chay sau khi da tao tai khoan, dung de chuyen mat khau plain text -> hash
UPDATE TaiKhoan SET MatKhau = N'wNqykWOAM5hGb8+mPUYvaw==.JpcQKcXxRJl31glDE1nu6/PUQsZAxLvS5j1YHAwxLT4=' WHERE TenDangNhap = N'fntzzs682@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'0OUCfE9U3HuDS2VghvJd3g==.FNV/tRowCFfYXFBgTlPgmDYep6OCLtZyQEArjrxE4Vg=' WHERE TenDangNhap = N'minhquanmkp123@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'jfj5pQ4ZwoZuOVn4pZCu1Q==.aDRo1vo42rnvA2i/E+B4kqvHddiwNog1Ztj/3tY7S1k=' WHERE TenDangNhap = N'Ngttu2006@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly1';
UPDATE TaiKhoan SET MatKhau = N'W/91Lmc0TaaA7wHTc+7Vvg==.usPMQz72I2LW9aN2XIPpxGP57LfvypZ+MPQPXWrxLmI=' WHERE TenDangNhap = N'thanhdac223@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'n2zN5/JWq7EppQ1RgTIZ4g==.HeZXQln7bNEKqIg6tC+U5w2z6N2SZiQ+wJI41bpdvYg=' WHERE TenDangNhap = N'user1';
UPDATE TaiKhoan SET MatKhau = N'NErALrnWhGZIsGdyZMHueg==.qC0xjUR/NjlOtz8ZdBImkgq8qxDt9elxyC7hIyF2p/E=' WHERE TenDangNhap = N'Vanh280306@gmail.com';