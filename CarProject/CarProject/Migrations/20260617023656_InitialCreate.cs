using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiTietKhachHang",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKhachHang", x => x.MaKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "ChuongTrinhKhuyenMai",
                columns: table => new
                {
                    MaKhuyenMai = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiGiamGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MucGiamToiDa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuongTrinhKhuyenMai", x => x.MaKhuyenMai);
                });

            migrationBuilder.CreateTable(
                name: "HangXe",
                columns: table => new
                {
                    MaHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuocGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanLogo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangXe", x => x.MaHang);
                });

            migrationBuilder.CreateTable(
                name: "KenhTuVan",
                columns: table => new
                {
                    MaKenh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlMessenger = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlZalo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlSMS = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KenhTuVan", x => x.MaKenh);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "DongXe",
                columns: table => new
                {
                    MaDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHang = table.Column<int>(type: "int", nullable: false),
                    TenDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KieuDang = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DongXe", x => x.MaDong);
                    table.ForeignKey(
                        name: "FK_DongXe_HangXe_MaHang",
                        column: x => x.MaHang,
                        principalTable: "HangXe",
                        principalColumn: "MaHang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiNhanhShowroom",
                columns: table => new
                {
                    MaChiNhanh = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenChiNhanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThanhPho = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDayNong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaQuanLy = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiNhanhShowroom", x => x.MaChiNhanh);
                    table.ForeignKey(
                        name: "FK_ChiNhanhShowroom_TaiKhoan_MaQuanLy",
                        column: x => x.MaQuanLy,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuangCaoBanner",
                columns: table => new
                {
                    MaBanner = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanLienKet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuTuHienThi = table.Column<int>(type: "int", nullable: false),
                    MaQuanLyCapNhat = table.Column<int>(type: "int", nullable: false),
                    TrangThaiKichHoat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuangCaoBanner", x => x.MaBanner);
                    table.ForeignKey(
                        name: "FK_QuangCaoBanner_TaiKhoan_MaQuanLyCapNhat",
                        column: x => x.MaQuanLyCapNhat,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhienBanXe_SanPham",
                columns: table => new
                {
                    MaPhienBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDong = table.Column<int>(type: "int", nullable: false),
                    TenPhienBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaNiemYet = table.Column<long>(type: "bigint", nullable: false),
                    MauSac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DongCo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HopSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiNhietLieu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLuongTrongKho = table.Column<int>(type: "int", nullable: false),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaKhuyenMai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhienBanXe_SanPham", x => x.MaPhienBan);
                    table.ForeignKey(
                        name: "FK_PhienBanXe_SanPham_DongXe_MaDong",
                        column: x => x.MaDong,
                        principalTable: "DongXe",
                        principalColumn: "MaDong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichHenLaiThu",
                columns: table => new
                {
                    MaLichHen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaDong = table.Column<int>(type: "int", nullable: false),
                    MaChiNhanh = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTenNguoiLai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoBangLaiXe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioHen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YKienKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHenLaiThu", x => x.MaLichHen);
                    table.ForeignKey(
                        name: "FK_LichHenLaiThu_ChiNhanhShowroom_MaChiNhanh",
                        column: x => x.MaChiNhanh,
                        principalTable: "ChiNhanhShowroom",
                        principalColumn: "MaChiNhanh",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichHenLaiThu_DongXe_MaDong",
                        column: x => x.MaDong,
                        principalTable: "DongXe",
                        principalColumn: "MaDong",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichHenLaiThu_TaiKhoan_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "ThongKeTongHop_Boss",
                columns: table => new
                {
                    MaThongKe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KyBaoCao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChiNhanh = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TongDoanhThu = table.Column<long>(type: "bigint", nullable: false),
                    TongTienCocThuVe = table.Column<long>(type: "bigint", nullable: false),
                    TongSoXeDaBan = table.Column<int>(type: "int", nullable: false),
                    SoDonCocBiHuy = table.Column<int>(type: "int", nullable: false),
                    TongLuotXemWeb = table.Column<int>(type: "int", nullable: false),
                    TongLuotLaiThu = table.Column<int>(type: "int", nullable: false),
                    MaDongXeBanChayNhat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeTongHop_Boss", x => x.MaThongKe);
                    table.ForeignKey(
                        name: "FK_ThongKeTongHop_Boss_ChiNhanhShowroom_MaChiNhanh",
                        column: x => x.MaChiNhanh,
                        principalTable: "ChiNhanhShowroom",
                        principalColumn: "MaChiNhanh",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThongKeTongHop_Boss_DongXe_MaDongXeBanChayNhat",
                        column: x => x.MaDongXeBanChayNhat,
                        principalTable: "DongXe",
                        principalColumn: "MaDong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonDatCoc",
                columns: table => new
                {
                    MaDonCoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaPhienBan = table.Column<int>(type: "int", nullable: false),
                    MaQuanLyDuyet = table.Column<int>(type: "int", nullable: true),
                    SoTienCoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTaoDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHenNhanXe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiDonHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonDatCoc", x => x.MaDonCoc);
                    table.ForeignKey(
                        name: "FK_DonDatCoc_PhienBanXe_SanPham_MaPhienBan",
                        column: x => x.MaPhienBan,
                        principalTable: "PhienBanXe_SanPham",
                        principalColumn: "MaPhienBan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonDatCoc_TaiKhoan_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonDatCoc_TaiKhoan_MaQuanLyDuyet",
                        column: x => x.MaQuanLyDuyet,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "HoaDonMuaXe",
                columns: table => new
                {
                    MaHoaDon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaDonCoc = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaPhienBan = table.Column<int>(type: "int", nullable: false),
                    MaQuanLyXuat = table.Column<int>(type: "int", nullable: false),
                    GiaXeThucTe = table.Column<long>(type: "bigint", nullable: false),
                    ThueTruocBaVaPhiLanBanh = table.Column<long>(type: "bigint", nullable: false),
                    SoTienDuocGiam = table.Column<long>(type: "bigint", nullable: false),
                    TongTienPhaiTra = table.Column<long>(type: "bigint", nullable: false),
                    SoTienDaThanhToan = table.Column<long>(type: "bigint", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayXuatHoaDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoKhung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiHoaDon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonMuaXe", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDonMuaXe_DonDatCoc_MaDonCoc",
                        column: x => x.MaDonCoc,
                        principalTable: "DonDatCoc",
                        principalColumn: "MaDonCoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiNhanhShowroom_MaQuanLy",
                table: "ChiNhanhShowroom",
                column: "MaQuanLy");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatCoc_MaKhachHang",
                table: "DonDatCoc",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatCoc_MaPhienBan",
                table: "DonDatCoc",
                column: "MaPhienBan");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatCoc_MaQuanLyDuyet",
                table: "DonDatCoc",
                column: "MaQuanLyDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_DongXe_MaHang",
                table: "DongXe",
                column: "MaHang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonMuaXe_MaDonCoc",
                table: "HoaDonMuaXe",
                column: "MaDonCoc");

            migrationBuilder.CreateIndex(
                name: "IX_LichHenLaiThu_MaChiNhanh",
                table: "LichHenLaiThu",
                column: "MaChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_LichHenLaiThu_MaDong",
                table: "LichHenLaiThu",
                column: "MaDong");

            migrationBuilder.CreateIndex(
                name: "IX_LichHenLaiThu_MaKhachHang",
                table: "LichHenLaiThu",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_PhienBanXe_SanPham_MaDong",
                table: "PhienBanXe_SanPham",
                column: "MaDong");

            migrationBuilder.CreateIndex(
                name: "IX_QuangCaoBanner_MaQuanLyCapNhat",
                table: "QuangCaoBanner",
                column: "MaQuanLyCapNhat");

            migrationBuilder.CreateIndex(
                name: "IX_ThongKeTongHop_Boss_MaChiNhanh",
                table: "ThongKeTongHop_Boss",
                column: "MaChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_ThongKeTongHop_Boss_MaDongXeBanChayNhat",
                table: "ThongKeTongHop_Boss",
                column: "MaDongXeBanChayNhat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietKhachHang");

            migrationBuilder.DropTable(
                name: "ChuongTrinhKhuyenMai");

            migrationBuilder.DropTable(
                name: "HoaDonMuaXe");

            migrationBuilder.DropTable(
                name: "KenhTuVan");

            migrationBuilder.DropTable(
                name: "LichHenLaiThu");

            migrationBuilder.DropTable(
                name: "QuangCaoBanner");

            migrationBuilder.DropTable(
                name: "ThongKeTongHop_Boss");

            migrationBuilder.DropTable(
                name: "DonDatCoc");

            migrationBuilder.DropTable(
                name: "ChiNhanhShowroom");

            migrationBuilder.DropTable(
                name: "PhienBanXe_SanPham");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "DongXe");

            migrationBuilder.DropTable(
                name: "HangXe");
        }
    }
}
