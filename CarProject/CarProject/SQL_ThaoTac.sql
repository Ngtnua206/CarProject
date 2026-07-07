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

-- Chi nhánh
SELECT * FROM ChiNhanhShowroom ORDER BY MaChiNhanh;

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

select * from TaiKhoan;

-- ==================== THÊM DỮ LIỆU ====================

-- Thêm hãng xe (MaHang tự tăng)
INSERT INTO HangXe (TenHang, QuocGia, DuongDanLogo)
VALUES ('Mercedes-Benz', 'Đức', '/images/brands/mercedes.png');

-- Thêm dòng xe (MaHang lấy từ bảng HangXe)
INSERT INTO DongXe (MaHang, TenDong, KieuDang)
VALUES (1, 'Mercedes C-Class', 'Sedan');

-- Thêm phiên bản xe (MaDong lấy từ bảng DongXe)
INSERT INTO PhienBanXe_SanPham (MaDong, TenPhienBan, GiaNiemYet, MauSac, DongCo, HopSo, LoaiNhietLieu, SoLuongTrongKho, DuongDanAnh, TrangThai)
VALUES (1, 'C200 AMG', 1500000000, 'Đen', '2.0L Turbo', '9G-Tronic', 'Xăng', 5, '/images/cars/c200.jpg', 'Còn hàng');

-- Thêm tài khoản
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai)
VALUES ('admin', 'admin123', 'Admin', 'Active');

-- Thêm chi nhánh (MaQuanLy = TenDangNhap của tài khoản quản lý)
INSERT INTO ChiNhanhShowroom (MaChiNhanh, TenChiNhanh, DiaChi, ThanhPho, DuongDayNong, MaQuanLy, TrangThai)
VALUES ('CN01', 'Showroom Sài Gòn', '123 Nguyễn Văn Linh Quận 7', 'TP.HCM', '0909123456', 'admin', 'Hoạt động');

-- Thêm khuyến mãi
INSERT INTO ChuongTrinhKhuyenMai (MaKhuyenMai, TieuDe, MoTa, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, NgayBatDau, NgayKetThuc, TrangThai)
VALUES ('KM01', 'Giảm 50% lệ phí trước bạ', 'Áp dụng cho tất cả dòng xe', 'Phần trăm', 50, 50000000, '2024-01-01', '2024-03-31', 'Hoạt động');

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
UPDATE PhienBanXe_SanPham SET TrangThai = 'Hết hàng' WHERE MaPhienBan = 1;

-- Đổi quyền tài khoản
UPDATE TaiKhoan SET VaiTro = 'Quản Lý' WHERE TenDangNhap = 'user';

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
DELETE FROM TaiKhoan WHERE TenDangNhap = 'user';


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
