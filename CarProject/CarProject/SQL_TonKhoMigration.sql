BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729111305_AddTonKhoTheoChiNhanh'
)
BEGIN
    CREATE TABLE [TonKhoTheoChiNhanh] (
        [MaTonKho] int NOT NULL IDENTITY,
        [MaPhienBan] int NOT NULL,
        [MaChiNhanh] nvarchar(450) NOT NULL,
        [SoLuong] int NOT NULL,
        [NgayCapNhat] datetime2 NOT NULL,
        CONSTRAINT [PK_TonKhoTheoChiNhanh] PRIMARY KEY ([MaTonKho]),
        CONSTRAINT [FK_TonKhoTheoChiNhanh_ChiNhanhShowroom_MaChiNhanh] FOREIGN KEY ([MaChiNhanh]) REFERENCES [ChiNhanhShowroom] ([MaChiNhanh]),
        CONSTRAINT [FK_TonKhoTheoChiNhanh_PhienBanXe_SanPham_MaPhienBan] FOREIGN KEY ([MaPhienBan]) REFERENCES [PhienBanXe_SanPham] ([MaPhienBan]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729111305_AddTonKhoTheoChiNhanh'
)
BEGIN
    CREATE INDEX [IX_TonKhoTheoChiNhanh_MaChiNhanh] ON [TonKhoTheoChiNhanh] ([MaChiNhanh]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729111305_AddTonKhoTheoChiNhanh'
)
BEGIN
    CREATE INDEX [IX_TonKhoTheoChiNhanh_MaPhienBan] ON [TonKhoTheoChiNhanh] ([MaPhienBan]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729111305_AddTonKhoTheoChiNhanh'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729111305_AddTonKhoTheoChiNhanh', N'8.0.0');
END;
GO

COMMIT;
GO

