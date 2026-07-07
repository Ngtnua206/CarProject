using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages;

public class CompareModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public CompareModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public List<DongXe> DongXeList { get; set; } = new();
    public List<DongXe> AllCars { get; set; } = new();
    public int Id1 { get; set; }
    public int Id2 { get; set; }
    public int Id3 { get; set; }

    public async Task OnGetAsync(int? id1, int? id2, int? id3)
    {
        AllCars = await _db.DongXe.Include(d => d.HangXe).ToListAsync();

        var ids = new[] { id1, id2, id3 }.Where(id => id.HasValue && id.Value > 0).Select(id => id!.Value).ToList();
        if (ids.Any())
        {
            DongXeList = await _db.DongXe.Include(d => d.HangXe)
                .Where(d => ids.Contains(d.MaDong)).ToListAsync();
            Id1 = id1 ?? 0;
            Id2 = id2 ?? 0;
            Id3 = id3 ?? 0;
        }

        await _log.LogAsync("Xem trang so sánh xe");
    }
}
