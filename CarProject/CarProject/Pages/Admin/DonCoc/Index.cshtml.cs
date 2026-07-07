using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.DonCoc;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    public List<DonDatCoc> DonCocList { get; set; }

    public IndexModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task OnGetAsync()
    {
        DonCocList = await _db.DonDatCoc
            .Include(d => d.KhachHang)
            .Include(d => d.PhienBan)
            .OrderByDescending(d => d.NgayTaoDon)
            .ToListAsync();
        await _log.LogAsync("Admin Xem danh sách đơn cọc");
    }
}
