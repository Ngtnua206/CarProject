using Microsoft.EntityFrameworkCore;
using CarProject.Models;

namespace CarProject.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<HangXe> HangXe { get; set; }
    public DbSet<DongXe> DongXe { get; set; }
    public DbSet<PhienBanXe> PhienBanXe { get; set; }
    public DbSet<TaiKhoan> TaiKhoan { get; set; }
    public DbSet<ChiTietKhachHang> ChiTietKhachHang { get; set; }
    public DbSet<DonDatCoc> DonDatCoc { get; set; }
    public DbSet<HoaDonMuaXe> HoaDonMuaXe { get; set; }
    public DbSet<LichHenLaiThu> LichHenLaiThu { get; set; }
    public DbSet<ChiNhanhShowroom> ChiNhanhShowroom { get; set; }
    public DbSet<ChuongTrinhKhuyenMai> ChuongTrinhKhuyenMai { get; set; }
    public DbSet<QuangCaoBanner> QuangCaoBanner { get; set; }
    public DbSet<KenhTuVan> KenhTuVan { get; set; }
    public DbSet<ThongKeTongHop_Boss> ThongKeTongHop_Boss { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HangXe>().ToTable("HangXe");
        modelBuilder.Entity<DongXe>().ToTable("DongXe");
        modelBuilder.Entity<PhienBanXe>().ToTable("PhienBanXe_SanPham");

        modelBuilder.Entity<HangXe>().HasKey(h => h.MaHang);
        modelBuilder.Entity<DongXe>().HasKey(d => d.MaDong);
        modelBuilder.Entity<PhienBanXe>().HasKey(p => p.MaPhienBan);

        modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
        modelBuilder.Entity<TaiKhoan>().HasKey(t => t.MaTaiKhoan);

        modelBuilder.Entity<ChiTietKhachHang>().ToTable("ChiTietKhachHang");
        modelBuilder.Entity<ChiTietKhachHang>().HasKey(c => c.MaKhachHang);

        modelBuilder.Entity<DonDatCoc>().ToTable("DonDatCoc");
        modelBuilder.Entity<DonDatCoc>().HasKey(d => d.MaDonCoc);

        modelBuilder.Entity<HoaDonMuaXe>().ToTable("HoaDonMuaXe");
        modelBuilder.Entity<HoaDonMuaXe>().HasKey(hd => hd.MaHoaDon);

        modelBuilder.Entity<LichHenLaiThu>().ToTable("LichHenLaiThu");
        modelBuilder.Entity<LichHenLaiThu>().HasKey(l => l.MaLichHen);

        modelBuilder.Entity<ChiNhanhShowroom>().ToTable("ChiNhanhShowroom");
        modelBuilder.Entity<ChiNhanhShowroom>().HasKey(cn => cn.MaChiNhanh);

        modelBuilder.Entity<ChuongTrinhKhuyenMai>().ToTable("ChuongTrinhKhuyenMai");
        modelBuilder.Entity<ChuongTrinhKhuyenMai>().HasKey(k => k.MaKhuyenMai);

        modelBuilder.Entity<QuangCaoBanner>().ToTable("QuangCaoBanner");
        modelBuilder.Entity<QuangCaoBanner>().HasKey(q => q.MaBanner);

        modelBuilder.Entity<KenhTuVan>().ToTable("KenhTuVan");
        modelBuilder.Entity<KenhTuVan>().HasKey(k => k.MaKenh);

        modelBuilder.Entity<ThongKeTongHop_Boss>().ToTable("ThongKeTongHop_Boss");
        modelBuilder.Entity<ThongKeTongHop_Boss>().HasKey(t => t.MaThongKe);

        modelBuilder.Entity<DongXe>()
            .HasOne(d => d.HangXe)
            .WithMany()
            .HasForeignKey(d => d.MaHang)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PhienBanXe>()
            .HasOne(p => p.DongXe)
            .WithMany()
            .HasForeignKey(p => p.MaDong)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LichHenLaiThu>()
            .HasOne(l => l.KhachHang)
            .WithMany()
            .HasForeignKey(l => l.MaKhachHang)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
