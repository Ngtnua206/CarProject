using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;
using System.Text.Json;

namespace CarProject.Pages.Admin.ThongKe;

public class DoanhThuModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty(SupportsGet = true)]
    public DateTime? TuNgay { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DenNgay { get; set; }

    [BindProperty(SupportsGet = true)]
    public string MaChiNhanh { get; set; }

    public List<ChiNhanhShowroom> DanhSachChiNhanh { get; set; }
    public string JsonLabels { get; set; }
    public string JsonData { get; set; }
    public string JsonDataChiNhanh { get; set; }
    public long TongDoanhThu { get; set; }
    public int TongDon { get; set; }
    public List<DoanhThuTheoChiNhanh> DoanhThuChiNhanh { get; set; } = new();

    public DoanhThuModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task OnGetAsync()
    {
        var homNay = DateTime.Today;
        TuNgay ??= homNay.AddDays(-30);
        DenNgay ??= homNay;

        DanhSachChiNhanh = await _db.ChiNhanhShowroom
            .Where(c => c.TrangThai == "Hoạt động")
            .ToListAsync();

        // Base query: DonDatCoc trong khoảng ngày
        IQueryable<DonDatCoc> query = _db.DonDatCoc
            .Where(d => d.NgayTaoDon >= TuNgay.Value && d.NgayTaoDon <= DenNgay.Value.AddDays(1));

        // Filter by showroom if specified
        if (!string.IsNullOrEmpty(MaChiNhanh))
        {
            // Filter by manager of that showroom
            var managerOfShowroom = await _db.ChiNhanhShowroom
                .Where(c => c.MaChiNhanh == MaChiNhanh)
                .Select(c => c.MaQuanLy)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(managerOfShowroom))
                query = query.Where(d => d.MaQuanLyDuyet == managerOfShowroom);
        }

        // Daily revenue
        var dailyData = await query
            .GroupBy(d => d.NgayTaoDon.Date)
            .Select(g => new { Ngay = g.Key, Tong = g.Sum(d => (long?)d.SoTienCoc) ?? 0 })
            .OrderBy(g => g.Ngay)
            .ToListAsync();

        // Fill missing dates
        var labels = new List<string>();
        var values = new List<long>();
        var dataDict = dailyData.ToDictionary(d => d.Ngay, d => d.Tong);

        for (var date = TuNgay.Value.Date; date <= DenNgay.Value.Date; date = date.AddDays(1))
        {
            labels.Add(date.ToString("dd/MM"));
            values.Add(dataDict.GetValueOrDefault(date, 0));
        }

        JsonLabels = JsonSerializer.Serialize(labels);
        JsonData = JsonSerializer.Serialize(values);

        // Totals
        TongDoanhThu = await query.SumAsync(d => (long?)d.SoTienCoc) ?? 0;
        TongDon = await query.CountAsync();

        // Revenue by showroom
        var chiNhanhList = await _db.ChiNhanhShowroom
            .Where(c => c.TrangThai == "Hoạt động" && c.MaQuanLy != null)
            .ToListAsync();

        var showroomRevenueDict = new Dictionary<string, long>();
        foreach (var cn in chiNhanhList)
        {
            var dt = await _db.DonDatCoc
                .Where(d => d.MaQuanLyDuyet == cn.MaQuanLy
                    && d.NgayTaoDon >= TuNgay.Value
                    && d.NgayTaoDon <= DenNgay.Value.AddDays(1))
                .SumAsync(d => (long?)d.SoTienCoc) ?? 0;
            showroomRevenueDict[cn.TenChiNhanh] = dt;
            DoanhThuChiNhanh.Add(new DoanhThuTheoChiNhanh
            {
                TenChiNhanh = cn.TenChiNhanh,
                TongDoanhThu = dt
            });
        }

        JsonDataChiNhanh = JsonSerializer.Serialize(DoanhThuChiNhanh
            .Select(d => new { label = d.TenChiNhanh, value = d.TongDoanhThu }));

        await _log.LogAsync("Admin xem doanh thu",
            $"từ {TuNgay:dd/MM/yyyy} đến {DenNgay:dd/MM/yyyy}" +
            (string.IsNullOrEmpty(MaChiNhanh) ? "" : $" - chi nhánh {MaChiNhanh}"));
    }
}

public class DoanhThuTheoChiNhanh
{
    public string TenChiNhanh { get; set; }
    public long TongDoanhThu { get; set; }
}
