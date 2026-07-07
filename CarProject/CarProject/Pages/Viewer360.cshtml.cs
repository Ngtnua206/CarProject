using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages;

public class Viewer360Model : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public Viewer360Model(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public List<DongXe> DanhSachXe { get; set; } = new();
    public DongXe? SelectedXe { get; set; }
    public int SelectedId { get; set; }

    public async Task OnGetAsync(int? id)
    {
        DanhSachXe = await _db.DongXe.Include(d => d.HangXe).ToListAsync();

        if (id.HasValue)
        {
            SelectedXe = await _db.DongXe.Include(d => d.HangXe)
                .FirstOrDefaultAsync(d => d.MaDong == id.Value);
            SelectedId = id.Value;
        }

        await _log.LogAsync($"Xem 360: {(SelectedXe?.TenDong ?? "không chọn")}");
    }
}
