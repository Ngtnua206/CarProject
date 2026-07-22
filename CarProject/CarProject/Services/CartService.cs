using System.Text.Json;
using CarProject.Data;
using CarProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CarProject.Services;

public class CartItem
{
    public int MaPhienBan { get; set; }
    public string TenPhienBan { get; set; } = "";
    public string TenDong { get; set; } = "";
    public long GiaNiemYet { get; set; }
    public decimal SoTienCoc { get; set; }
    public string? MauSac { get; set; }
    public string? DongCo { get; set; }
    public string? HopSo { get; set; }
    public string? DuongDanAnh { get; set; }
    public int SoLuong { get; set; } = 1;
}

public interface ICartService
{
    Task<List<CartItem>> GetCartAsync();
    Task AddToCartAsync(CartItem item);
    Task RemoveFromCartAsync(int maPhienBan);
    Task UpdateQuantityAsync(int maPhienBan, int soLuong);
    Task ClearCartAsync();
    Task<int> GetCartCountAsync();
    Task<decimal> GetTotalDepositAsync();
    Task<bool> HasMinimumQuantityAsync(int min = 3);
}

public class CartService : ICartService
{
    private readonly IHttpContextAccessor _http;
    private readonly AppDbContext _db;

    public CartService(IHttpContextAccessor http, AppDbContext db)
    {
        _http = http;
        _db = db;
    }

    private string? GetUserId()
    {
        return _http.HttpContext?.User.GetJwtUserName();
    }

    public async Task<List<CartItem>> GetCartAsync()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return new List<CartItem>();

        var cartRows = await _db.GioHang
            .Where(g => g.MaTaiKhoan == userId)
            .Include(g => g.PhienBan).ThenInclude(p => p.DongXe)
            .ToListAsync();

        return cartRows.Select(g => new CartItem
        {
            MaPhienBan = g.MaPhienBan,
            SoLuong = g.SoLuong,
            TenPhienBan = g.PhienBan?.TenPhienBan ?? "",
            TenDong = g.PhienBan?.DongXe?.TenDong ?? "",
            GiaNiemYet = g.PhienBan?.GiaNiemYet ?? 0,
            SoTienCoc = Math.Round((g.PhienBan?.GiaNiemYet ?? 0) * 0.2m / 1_000_000) * 1_000_000,
            MauSac = g.PhienBan?.MauSac,
            DongCo = g.PhienBan?.DongCo,
            HopSo = g.PhienBan?.HopSo,
            DuongDanAnh = g.PhienBan?.DuongDanAnh
        }).ToList();
    }

    public async Task AddToCartAsync(CartItem item)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        var existing = await _db.GioHang
            .FirstOrDefaultAsync(g => g.MaTaiKhoan == userId && g.MaPhienBan == item.MaPhienBan);

        if (existing != null)
        {
            existing.SoLuong++;
        }
        else
        {
            _db.GioHang.Add(new GioHang
            {
                MaTaiKhoan = userId,
                MaPhienBan = item.MaPhienBan,
                SoLuong = 1,
                NgayTao = DateTime.Now
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(int maPhienBan)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        var rows = await _db.GioHang
            .Where(g => g.MaTaiKhoan == userId && g.MaPhienBan == maPhienBan)
            .ToListAsync();

        _db.GioHang.RemoveRange(rows);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateQuantityAsync(int maPhienBan, int soLuong)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        var existing = await _db.GioHang
            .FirstOrDefaultAsync(g => g.MaTaiKhoan == userId && g.MaPhienBan == maPhienBan);

        if (existing == null) return;

        if (soLuong <= 0)
            _db.GioHang.Remove(existing);
        else
            existing.SoLuong = soLuong;

        await _db.SaveChangesAsync();
    }

    public async Task ClearCartAsync()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return;

        var rows = await _db.GioHang.Where(g => g.MaTaiKhoan == userId).ToListAsync();
        _db.GioHang.RemoveRange(rows);
        await _db.SaveChangesAsync();
    }

    public async Task<int> GetCartCountAsync()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return 0;
        return await _db.GioHang.Where(g => g.MaTaiKhoan == userId).SumAsync(g => g.SoLuong);
    }

    public async Task<decimal> GetTotalDepositAsync()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return 0;

        var cartRows = await _db.GioHang
            .Where(g => g.MaTaiKhoan == userId)
            .Include(g => g.PhienBan)
            .ToListAsync();

        return cartRows.Sum(g =>
        {
            var deposit = Math.Round((g.PhienBan?.GiaNiemYet ?? 0) * 0.2m / 1_000_000) * 1_000_000;
            return deposit * g.SoLuong;
        });
    }

    public async Task<bool> HasMinimumQuantityAsync(int min = 3)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return false;
        var total = await _db.GioHang.Where(g => g.MaTaiKhoan == userId).SumAsync(g => g.SoLuong);
        return total >= min;
    }
}
