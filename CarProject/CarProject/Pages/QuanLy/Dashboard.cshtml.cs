using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.QuanLy;

public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public string TenDangNhap { get; set; }
    public string TenHienThi { get; set; }
    public ChiNhanhShowroom Showroom { get; set; }
    public long DoanhThuHomNay { get; set; }
    public int SoDonHomNay { get; set; }
    public long DoanhThuThangNay { get; set; }
    public int SoDonThangNay { get; set; }
    public int TongDon { get; set; }

    public List<DailyRevenue> DoanhThuTheoNgay { get; set; } = new();
    public string JsonLabels { get; set; }
    public string JsonData { get; set; }

    public DashboardModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userName = User.GetJwtUserName();
        var role = User.GetJwtRole();

        if (string.IsNullOrEmpty(userName))
            return RedirectToPage("/Account/Login");

        TenDangNhap = userName;
        TenHienThi = User.GetJwtDisplayName() ?? userName;

        // Tìm showroom được giao quản lý
        Showroom = await _db.ChiNhanhShowroom
            .FirstOrDefaultAsync(c => c.MaQuanLy == userName);

        if (Showroom == null)
            return Page();

        var homNay = DateTime.Today;
        var dauThang = new DateTime(homNay.Year, homNay.Month, 1);
        var truoc30Ngay = homNay.AddDays(-30);

        // Lấy các đơn cọc do quản lý này duyệt
        var donCocQuery = _db.DonDatCoc
            .Where(d => d.MaQuanLyDuyet == userName);

        DoanhThuHomNay = await donCocQuery
            .Where(d => d.NgayTaoDon >= homNay)
            .SumAsync(d => (long?)d.SoTienCoc) ?? 0;
        SoDonHomNay = await donCocQuery
            .Where(d => d.NgayTaoDon >= homNay)
            .CountAsync();

        DoanhThuThangNay = await donCocQuery
            .Where(d => d.NgayTaoDon >= dauThang)
            .SumAsync(d => (long?)d.SoTienCoc) ?? 0;
        SoDonThangNay = await donCocQuery
            .Where(d => d.NgayTaoDon >= dauThang)
            .CountAsync();

        TongDon = await donCocQuery.CountAsync();

        // Doanh thu 30 ngày gần nhất (cho biểu đồ)
        var dailyData = await donCocQuery
            .Where(d => d.NgayTaoDon >= truoc30Ngay)
            .GroupBy(d => d.NgayTaoDon.Date)
            .Select(g => new { Ngay = g.Key, Tong = g.Sum(d => (long?)d.SoTienCoc) ?? 0 })
            .OrderBy(g => g.Ngay)
            .ToListAsync();

        // Fill missing dates
        var labels = new List<string>();
        var values = new List<long>();
        var dataDict = dailyData.ToDictionary(d => d.Ngay, d => d.Tong);

        for (var date = truoc30Ngay.Date; date <= homNay; date = date.AddDays(1))
        {
            labels.Add(date.ToString("dd/MM"));
            values.Add(dataDict.GetValueOrDefault(date, 0));
        }

        JsonLabels = System.Text.Json.JsonSerializer.Serialize(labels);
        JsonData = System.Text.Json.JsonSerializer.Serialize(values);

        DoanhThuTheoNgay = dailyData.Select(d => new DailyRevenue
        {
            Ngay = d.Ngay,
            TongTien = d.Tong
        }).ToList();

        await _log.LogAsync("Quản Lý xem Dashboard", $"showroom {Showroom.TenChiNhanh}");
        return Page();
    }
}

public class DailyRevenue
{
    public DateTime Ngay { get; set; }
    public long TongTien { get; set; }
}
