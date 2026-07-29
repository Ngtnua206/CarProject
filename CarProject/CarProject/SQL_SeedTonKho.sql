-- Seed TonKhoTheoChiNhanh - phan bo ton kho cho tung showroom
-- Khong xoa du lieu cu, chi INSERT neu chua co

IF NOT EXISTS (SELECT 1 FROM TonKhoTheoChiNhanh)
BEGIN
    DECLARE @total INT = (SELECT COUNT(*) FROM PhienBanXe_SanPham);
    DECLARE @i INT = 1;

    WHILE @i <= @total
    BEGIN
        DECLARE @sl INT;
        SELECT @sl = SoLuongTrongKho FROM PhienBanXe_SanPham WHERE MaPhienBan = @i;

        IF @sl > 0
        BEGIN
            INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN01', @sl * 50 / 100, GETDATE());
            INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN02', @sl * 20 / 100, GETDATE());
            INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN03', @sl * 15 / 100, GETDATE());
            INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN04', @sl * 5 / 100, GETDATE());
            INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN05', @sl * 5 / 100, GETDATE());
            INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@i, 'CN06', @sl * 5 / 100, GETDATE());
        END

        SET @i = @i + 1;
    END

    DELETE FROM TonKhoTheoChiNhanh WHERE SoLuong = 0;

    DECLARE @j INT = 1;
    WHILE @j <= @total
    BEGIN
        DECLARE @hasRec INT;
        SELECT @hasRec = COUNT(*) FROM TonKhoTheoChiNhanh WHERE MaPhienBan = @j;
        IF @hasRec = 0
        BEGIN
            SELECT @sl = SoLuongTrongKho FROM PhienBanXe_SanPham WHERE MaPhienBan = @j;
            IF @sl > 0
                INSERT INTO TonKhoTheoChiNhanh (MaPhienBan, MaChiNhanh, SoLuong, NgayCapNhat) VALUES (@j, 'CN01', @sl, GETDATE());
        END
        SET @j = @j + 1;
    END
END
GO
