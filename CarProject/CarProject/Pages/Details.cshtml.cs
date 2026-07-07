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
        await _log.LogAsync("Xem chi tiết xe", $"{HangXe?.TenHang} {Dong.TenDong} (ID={id})");
        return Page();
    }
}
