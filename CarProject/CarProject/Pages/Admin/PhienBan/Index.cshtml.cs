using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.PhienBan;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    public List<PhienBanXe> PhienBanList { get; set; }

    public IndexModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task OnGetAsync()
    {
        PhienBanList = await _db.PhienBanXe.Include(p => p.DongXe).ToListAsync();
        await _log.LogAsync("Admin Xem danh sách phiên bản");
    }
}
