using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages;

public class CarsModel : PageModel
{
    private readonly AppDbContext _db;
    public List<DongXe> DongXeList { get; set; } = new();
    public List<HangXe> HangXeList { get; set; } = new();
    public int TotalCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Brand { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? BodyType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Sort { get; set; }

    public CarsModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGetAsync()
    {
        HangXeList = await _db.HangXe.ToListAsync();

        var query = _db.DongXe.Include(d => d.HangXe).AsQueryable();

        if (!string.IsNullOrEmpty(Search))
            query = query.Where(d => d.TenDong.Contains(Search));

        if (!string.IsNullOrEmpty(Brand) && int.TryParse(Brand, out var brandId))
            query = query.Where(d => d.MaHang == brandId);

        if (!string.IsNullOrEmpty(BodyType))
            query = query.Where(d => d.KieuDang == BodyType);

        query = Sort switch
        {
            "name_asc" => query.OrderBy(d => d.TenDong),
            "name_desc" => query.OrderByDescending(d => d.TenDong),
            _ => query.OrderBy(d => d.TenDong)
        };

        DongXeList = await query.ToListAsync();
        TotalCount = DongXeList.Count;
    }
}
