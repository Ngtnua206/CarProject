using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Services;

namespace CarProject.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public int TotalHangXe { get; set; }
    public int TotalDongXe { get; set; }
    public int TotalPhienBan { get; set; }
    public int TotalBanner { get; set; }
    public int TotalLogs { get; set; }
    public int TotalUsers { get; set; }

    public IndexModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task OnGetAsync()
    {
        // Check login
        if (!HttpContext.Session.GetInt32("UserId").HasValue)
        {
            RedirectToPage("/Account/Login");
            return;
        }

        TotalHangXe = await _db.HangXe.CountAsync();
        TotalDongXe = await _db.DongXe.CountAsync();
        TotalPhienBan = await _db.PhienBanXe.CountAsync();
        TotalBanner = await _db.QuangCaoBanner.CountAsync();
        TotalLogs = await _db.NhatKyHeThong.CountAsync();
        TotalUsers = await _db.TaiKhoan.CountAsync();

        await _log.LogAsync("Xem Admin Dashboard");
    }
}
