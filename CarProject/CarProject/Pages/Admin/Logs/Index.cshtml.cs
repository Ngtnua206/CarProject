using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.Logs;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public List<NhatKyHeThong> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int TotalPages => Math.Max(1, (TotalCount + PageSize - 1) / PageSize);

    [BindProperty(SupportsGet = true)]
    public string? SearchAction { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchUser { get; set; }

    public IndexModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task OnGetAsync(int? p)
    {
        if (!HttpContext.Session.GetInt32("UserId").HasValue)
        {
            RedirectToPage("/Account/Login");
            return;
        }

        PageIndex = p ?? 1;
        if (PageIndex < 1) PageIndex = 1;

        var query = _db.NhatKyHeThong.AsQueryable();
        var role = HttpContext.Session.GetString("UserRole") ?? "Guest";
        var userName = HttpContext.Session.GetString("UserName") ?? "";

        // Admin: xem tất cả; User thường: chỉ xem log của mình
        if (role != "Admin")
            query = query.Where(l => l.TenDangNhap == userName);

        if (!string.IsNullOrEmpty(SearchAction))
            query = query.Where(l => l.HanhDong.Contains(SearchAction));

        if (!string.IsNullOrEmpty(SearchUser) && role == "Admin")
            query = query.Where(l => l.TenDangNhap.Contains(SearchUser));

        TotalCount = await query.CountAsync();
        Logs = await query
            .OrderByDescending(l => l.ThoiGian)
            .Skip((PageIndex - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        await _log.LogAsync("Xem nhật ký hoạt động", $"Trang {PageIndex}/{TotalPages} - Tìm: \"{SearchAction}\" User: \"{SearchUser}\"");
    }
}
