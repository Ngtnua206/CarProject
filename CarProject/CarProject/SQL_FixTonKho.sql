-- Fix phan bo ton kho: dam bao tong ton kho theo showroom = SoLuongTrongKho
-- Xoa du lieu cu va tinh lai

DELETE FROM TonKhoTheoChiNhanh;

DECLARE @total INT = (SELECT COUNT(*) FROM PhienBanXe_SanPham);
DECLARE @i INT = 1;
DECLARE @sl INT;

WHILE @i <= @total
BEGIN
    SELECT @sl = SoLuongTrongKho FROM PhienBanXe_SanPham WHERE MaPhienBan = @i;

    IF @sl > 0
    BEGIN
        DECLARE @cn1 INT, @cn2 INT, @cn3 INT, @cn4 INT, @cn5 INT, @cn6 INT;

        SET @cn1 = @sl * 35 / 100;
        SET @cn2 = @sl * 25 / 100;
        SET @cn3 = @sl * 20 / 100;
        SET @cn4 = @sl * 10 / 100;
        SET @cn5 = @sl * 5 / 100;
        SET @cn6 = @sl - (@cn1 + @cn2 + @cn3 + @cn4 + @cn5);

        IF @cn6 < 0 SET @cn6 = 0;

        IF @cn1 > 0 INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN01', @cn1, GETDATE());
        IF @cn2 > 0 INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN02', @cn2, GETDATE());
        IF @cn3 > 0 INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN03', @cn3, GETDATE());
        IF @cn4 > 0 INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN04', @cn4, GETDATE());
        IF @cn5 > 0 INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN05', @cn5, GETDATE());
        IF @cn6 > 0 INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN06', @cn6, GETDATE());

        -- Neu khong co showroom nao duoc phan (all zero), cho het vao CN01
        IF NOT EXISTS (SELECT 1 FROM TonKhoTheoChiNhanh WHERE MaPhienBan = @i)
            INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN01', @sl, GETDATE());
    END

    SET @i = @i + 1;
END
GO
