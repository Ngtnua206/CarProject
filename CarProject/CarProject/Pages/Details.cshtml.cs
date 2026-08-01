using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    public DongXe Dong { get; set; }
    public HangXe HangXe { get; set; }
    public List<PhienBanXe> PhienBans { get; set; }
    public Dictionary<int, List<TonKhoTheoChiNhanh>> TonKhoTheoPhienBan { get; set; } = new();
    public Dictionary<int, Dictionary<string, int>> DaDatCocTheoPhienBanVaChiNhanh { get; set; } = new();
    public Dictionary<int, int> TongDaDatCocTheoPhienBan { get; set; } = new();

    public DetailsModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Dong = await _db.DongXe.Include(d => d.HangXe).FirstOrDefaultAsync(d => d.MaDong == id);
        if (Dong == null) return NotFound();
        HangXe = Dong.HangXe;
        PhienBans = await _db.PhienBanXe.Where(p => p.MaDong == id).ToListAsync();

        var maPhienBans = PhienBans.Select(p => p.MaPhienBan).ToList();
        var tonKhoList = await _db.TonKhoTheoChiNhanh
            .Include(t => t.ChiNhanh)
            .Where(t => maPhienBans.Contains(t.MaPhienBan))
            .ToListAsync();
        TonKhoTheoPhienBan = tonKhoList.GroupBy(t => t.MaPhienBan)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Các phiên bản đã được đặt cọc (đơn đang chờ xử lý/đã xác nhận) - đếm theo từng showroom
        var reservedStatuses = new List<string> { "Chờ xử lý", "Chờ xác nhận", "Đã xác nhận", "Đã thanh toán", "Hoàn tất" };
        var daDatCocList = await _db.DonDatCocChiTiet
            .Where(c => maPhienBans.Contains(c.MaPhienBan)
                && c.DonDatCoc != null
                && reservedStatuses.Contains(c.DonDatCoc!.TrangThaiDonHang ?? ""))
            .Select(c => new { c.MaPhienBan, c.MaChiNhanh, c.SoLuong })
            .ToListAsync();

        foreach (var x in daDatCocList)
        {
            if (!DaDatCocTheoPhienBanVaChiNhanh.TryGetValue(x.MaPhienBan, out var inner))
            {
                inner = new Dictionary<string, int>();
                DaDatCocTheoPhienBanVaChiNhanh[x.MaPhienBan] = inner;
            }
            var cnKey = x.MaChiNhanh ?? "";
            inner.TryGetValue(cnKey, out var cur);
            inner[cnKey] = cur + x.SoLuong;

            TongDaDatCocTheoPhienBan.TryGetValue(x.MaPhienBan, out var total);
            TongDaDatCocTheoPhienBan[x.MaPhienBan] = total + x.SoLuong;
        }

        await _log.LogAsync("Xem chi tiết xe", $"{HangXe?.TenHang} {Dong.TenDong} (ID={id})");
        return Page();
    }
}
