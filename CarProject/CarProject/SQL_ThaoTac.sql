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
select * from KenhTuVan;

-- ==================== THEM QUAN LY CHO MOI CHI NHANH ====================
-- Thêm Quản Lý cho CN02 (Thủ Đức)
IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = 'quanly2')
    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
    VALUES ('quanly2', 'quanly123', N'Quản Lý', N'Hoạt động', N'Nguyễn Văn B');
UPDATE ChiNhanhShowroom SET MaQuanLy = 'quanly2' WHERE MaChiNhanh = 'CN02';

-- Thêm Quản Lý cho CN03 (Cầu Giấy)
IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = 'quanly3')
    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
    VALUES ('quanly3', 'quanly123', N'Quản Lý', N'Hoạt động', N'Trần Thị C');
UPDATE ChiNhanhShowroom SET MaQuanLy = 'quanly3' WHERE MaChiNhanh = 'CN03';

-- Thêm Quản Lý cho CN04 (Hoàng Mai)
IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = 'quanly4')
    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
    VALUES ('quanly4', 'quanly123', N'Quản Lý', N'Hoạt động', N'Lê Văn D');
UPDATE ChiNhanhShowroom SET MaQuanLy = 'quanly4' WHERE MaChiNhanh = 'CN04';

-- Thêm Quản Lý cho CN05 (Đà Nẵng)
IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = 'quanly5')
    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
    VALUES ('quanly5', 'quanly123', N'Quản Lý', N'Hoạt động', N'Phạm Thị E');
UPDATE ChiNhanhShowroom SET MaQuanLy = 'quanly5' WHERE MaChiNhanh = 'CN05';

-- Thêm Quản Lý cho CN06 (Hải Phòng)
IF NOT EXISTS (SELECT 1 FROM TaiKhoan WHERE TenDangNhap = 'quanly6')
    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, TrangThai, TenHienThi)
    VALUES ('quanly6', 'quanly123', N'Quản Lý', N'Hoạt động', N'Hoàng Văn F');
UPDATE ChiNhanhShowroom SET MaQuanLy = 'quanly6' WHERE MaChiNhanh = 'CN06';

-- ==================== THEM DON DAT COC MAU ====================
-- Đơn cọc đã giao xe (doanh thu đã hoàn tất)
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250701-1')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 1, 'quanly1', 200000000, N'Chuyển khoản', N'Đã thanh toán', '2026-01-15 09:30:00', '2026-02-20', N'Đã giao xe', N'Giao tại showroom Quận 7', N'Nguyễn Văn A', '0901000001', N'123 Lê Lợi, Quận 1', 'MGC250701-1');
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250702-2')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 3, 'quanly2', 300000000, N'Chuyển khoản', N'Đã thanh toán', '2026-02-10 14:00:00', '2026-03-05', N'Đã giao xe', N'Xe màu bạc', N'Trần Văn B', '0901000002', N'456 Nguyễn Huệ, Quận 1', 'MGC250702-2');
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250703-3')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 5, 'quanly3', 150000000, N'Tiền mặt', N'Đã thanh toán', '2026-03-05 10:00:00', '2026-03-28', N'Đã giao xe', N'Giao tại showroom Cầu Giấy', N'Lê Thị C', '0901000003', N'789 Trần Hưng Đạo, Hoàn Kiếm', 'MGC250703-3');
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250704-4')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 7, 'quanly4', 450000000, N'Chuyển khoản', N'Đã thanh toán', '2026-03-20 15:30:00', '2026-04-15', N'Đã giao xe', N'Khách VIP - gói phụ kiện', N'Phạm Văn D', '0901000004', N'123 Nguyễn Văn Linh, Quận 7', 'MGC250704-4');

-- Đơn cọc đã xác nhận (chờ giao xe)
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250705-5')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 2, 'quanly5', 250000000, N'Chuyển khoản', N'Đã thanh toán', '2026-04-10 09:00:00', '2026-05-20', N'Đã xác nhận', N'Xe màu đen', N'Hoàng Thị E', '0901000005', N'456 Hải Phòng, Đà Nẵng', 'MGC250705-5');
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250706-6')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, NgayHenNhanXe, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 4, 'quanly6', 180000000, N'Tiền mặt', N'Đã thanh toán', '2026-05-05 11:30:00', '2026-06-10', N'Đã xác nhận', N'Giao tại showroom Hải Phòng', N'Đặng Văn F', '0901000006', N'789 Văn Cao, Hải Phòng', 'MGC250706-6');

-- Đơn cọc chờ xác nhận (chưa duyệt)
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250707-7')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 6, 'quanly1', 200000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-06-01 08:00:00', N'Chờ xác nhận', N'Khách đang chờ vay ngân hàng', N'Nguyễn Văn G', '0901000007', N'123 Quận 7, TP.HCM', 'MGC250707-7');
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250708-8')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 8, 'quanly2', 350000000, N'Chuyển khoản', N'Chưa thanh toán', '2026-06-15 14:00:00', N'Chờ xác nhận', N'Khách muốn lái thử trước', N'Trần Thị H', '0901000008', N'456 Thủ Đức, TP.HCM', 'MGC250708-8');

-- Đơn cọc bị hủy
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250709-9')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 9, 'quanly3', 100000000, N'Tiền mặt', N'Đã hoàn tiền', '2026-04-20 16:00:00', N'Đã hủy', N'Khách đổi ý không mua nữa', N'Lê Văn I', '0901000009', N'789 Cầu Giấy, Hà Nội', 'MGC250709-9');
IF NOT EXISTS (SELECT 1 FROM DonDatCoc WHERE MaGiaoDich = 'MGC250710-10')
    INSERT INTO DonDatCoc (MaKhachHang, MaPhienBan, MaQuanLyDuyet, SoTienCoc, PhuongThucThanhToan, TrangThaiThanhToan, NgayTaoDon, TrangThaiDonHang, GhiChu, HoTen, SoDienThoai, DiaChi, MaGiaoDich)
    VALUES ('user1', 10, 'quanly4', 150000000, N'Chuyển khoản', N'Đã hoàn tiền', '2026-05-10 10:30:00', N'Đã hủy', N'Không đủ khả năng tài chính', N'Phạm Thị K', '0901000010', N'321 Hoàng Mai, Hà Nội', 'MGC250710-10');

-- ==================== THEM HOA DON MAU ====================
-- Hóa đơn cho các đơn cọc đã giao xe
IF NOT EXISTS (SELECT 1 FROM HoaDonMuaXe WHERE MaHoaDon = 'HD000001')
    INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
    VALUES ('HD000001', 1, 'user1', 1, 'quanly1', 1050000000, 105000000, 50000000, 1105000000, 1105000000, N'Chuyển khoản + Tiền mặt', '2026-02-20', 'WDB1111111A000001', 'M274000001', N'Đã thanh toán');
IF NOT EXISTS (SELECT 1 FROM HoaDonMuaXe WHERE MaHoaDon = 'HD000002')
    INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
    VALUES ('HD000002', 2, 'user1', 3, 'quanly2', 950000000, 95000000, 30000000, 1015000000, 1015000000, N'Chuyển khoản', '2026-03-05', 'WDB2222222A000002', 'M274000002', N'Đã thanh toán');
IF NOT EXISTS (SELECT 1 FROM HoaDonMuaXe WHERE MaHoaDon = 'HD000003')
    INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
    VALUES ('HD000003', 3, 'user1', 5, 'quanly3', 1300000000, 130000000, 80000000, 1350000000, 1350000000, N'Chuyển khoản', '2026-03-28', 'WDB3333333A000003', 'M274000003', N'Đã thanh toán');
IF NOT EXISTS (SELECT 1 FROM HoaDonMuaXe WHERE MaHoaDon = 'HD000004')
    INSERT INTO HoaDonMuaXe (MaHoaDon, MaDonCoc, MaKhachHang, MaPhienBan, MaQuanLyXuat, GiaXeThucTe, ThueTruocBaVaPhiLanBanh, SoTienDuocGiam, TongTienPhaiTra, SoTienDaThanhToan, PhuongThucThanhToan, NgayXuatHoaDon, SoKhung, SoMay, TrangThaiHoaDon)
    VALUES ('HD000004', 4, 'user1', 7, 'quanly4', 1600000000, 160000000, 120000000, 1640000000, 1640000000, N'Chuyển khoản', '2026-04-15', 'WDB4444444A000004', 'M274000004', N'Đã thanh toán');

-- ==================== THEM THONG KE DOANH THU ====================
-- Xóa thống kê cũ (nếu có) để insert lại
DELETE FROM ThongKeTongHop_Boss;

-- Thống kê theo tháng cho từng chi nhánh (2026)
-- Tháng 1
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN01', 1105000000, 200000000, 1, 0, 15000, 45, 1);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN02', 500000000, 80000000, 0, 1, 12000, 30, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN03', 850000000, 150000000, 0, 0, 9000, 25, 5);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN04', 300000000, 50000000, 0, 0, 8000, 20, 7);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-01', 'CN05', 200000000, 30000000, 0, 0, 5000, 12, 2);

-- Tháng 2
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN01', 2100000000, 300000000, 2, 0, 18000, 55, 1);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN02', 1015000000, 300000000, 1, 0, 14000, 35, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN03', 600000000, 100000000, 0, 0, 10000, 28, 5);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-02', 'CN06', 450000000, 80000000, 0, 0, 6000, 15, 4);

-- Tháng 3
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN01', 3100000000, 500000000, 3, 0, 22000, 65, 1);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN02', 1800000000, 250000000, 1, 1, 16000, 40, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN03', 1350000000, 150000000, 1, 0, 12000, 32, 5);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN04', 1640000000, 450000000, 1, 0, 11000, 30, 7);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-03', 'CN05', 800000000, 250000000, 0, 0, 7000, 18, 2);

-- Tháng 4
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN01', 2500000000, 350000000, 2, 0, 20000, 60, 1);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN02', 1400000000, 200000000, 1, 0, 15000, 38, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN05', 1200000000, 250000000, 1, 0, 8000, 22, 2);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-04', 'CN06', 700000000, 100000000, 0, 0, 7000, 16, 4);

-- Tháng 5
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN01', 2800000000, 400000000, 2, 1, 24000, 70, 1);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN02', 2000000000, 350000000, 1, 0, 18000, 45, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN03', 1600000000, 200000000, 1, 1, 14000, 35, 5);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-05', 'CN06', 900000000, 180000000, 0, 0, 8000, 20, 4);

-- Tháng 6
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN01', 3200000000, 500000000, 2, 0, 26000, 75, 1);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN02', 2200000000, 350000000, 2, 0, 20000, 50, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN03', 1500000000, 200000000, 1, 0, 16000, 40, 5);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN04', 1800000000, 300000000, 1, 0, 14000, 35, 7);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN05', 1300000000, 250000000, 1, 0, 10000, 28, 2);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-06', 'CN06', 1000000000, 200000000, 0, 0, 9000, 22, 4);

-- Tháng 7 (hiện tại, dang dở)
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-07', 'CN01', 1500000000, 200000000, 1, 0, 12000, 35, 1);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-07', 'CN02', 800000000, 100000000, 0, 0, 8000, 20, 3);
INSERT INTO ThongKeTongHop_Boss (KyBaoCao, MaChiNhanh, TongDoanhThu, TongTienCocThuVe, TongSoXeDaBan, SoDonCocBiHuy, TongLuotXemWeb, TongLuotLaiThu, MaDongXeBanChayNhat)
VALUES (N'2026-07', 'CN03', 600000000, 80000000, 0, 0, 6000, 15, 5);

-- ==================== UPDATE MAT KHAU CHO CAC QUAN LY MOI ====================
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly2' AND MatKhau = 'quanly123';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly3' AND MatKhau = 'quanly123';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly4' AND MatKhau = 'quanly123';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly5' AND MatKhau = 'quanly123';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly6' AND MatKhau = 'quanly123';

-- ==================== CAP NHAT MAT KHAU (PBKDF2 hash) ====================
-- Chay sau khi da tao tai khoan, dung de chuyen mat khau plain text -> hash
UPDATE TaiKhoan SET MatKhau = N'wNqykWOAM5hGb8+mPUYvaw==.JpcQKcXxRJl31glDE1nu6/PUQsZAxLvS5j1YHAwxLT4=' WHERE TenDangNhap = N'fntzzs682@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'0OUCfE9U3HuDS2VghvJd3g==.FNV/tRowCFfYXFBgTlPgmDYep6OCLtZyQEArjrxE4Vg=' WHERE TenDangNhap = N'minhquanmkp123@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'jfj5pQ4ZwoZuOVn4pZCu1Q==.aDRo1vo42rnvA2i/E+B4kqvHddiwNog1Ztj/3tY7S1k=' WHERE TenDangNhap = N'Ngttu2006@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'f1LPcH537MYN7s5VbzoamQ==.irSEdtlKdnMK1UQHBm/SKs75n4tls9HQXC+Gf+MXmTc=' WHERE TenDangNhap = N'quanly1';
UPDATE TaiKhoan SET MatKhau = N'W/91Lmc0TaaA7wHTc+7Vvg==.usPMQz72I2LW9aN2XIPpxGP57LfvypZ+MPQPXWrxLmI=' WHERE TenDangNhap = N'thanhdac223@gmail.com';
UPDATE TaiKhoan SET MatKhau = N'n2zN5/JWq7EppQ1RgTIZ4g==.HeZXQln7bNEKqIg6tC+U5w2z6N2SZiQ+wJI41bpdvYg=' WHERE TenDangNhap = N'user1';
UPDATE TaiKhoan SET MatKhau = N'NErALrnWhGZIsGdyZMHueg==.qC0xjUR/NjlOtz8ZdBImkgq8qxDt9elxyC7hIyF2p/E=' WHERE TenDangNhap = N'Vanh280306@gmail.com';