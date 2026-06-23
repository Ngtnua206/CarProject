using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public List<HangXe> HangXeList { get; set; } = new();
    public List<DongXe> DongXeList { get; set; } = new();
    public int TotalModels { get; set; }
    public int TotalBrands { get; set; }
    public int TotalVersions { get; set; }

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGetAsync()
    {
        HangXeList = await _db.HangXe.ToListAsync();
        DongXeList = await _db.DongXe.Include(d => d.HangXe).ToListAsync();
        TotalBrands = HangXeList.Count;
        TotalModels = DongXeList.Count;
        TotalVersions = await _db.PhienBanXe.CountAsync();
    }
}
