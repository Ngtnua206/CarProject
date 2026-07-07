using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.Banner;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public QuangCaoBanner Banner { get; set; }

    public CreateModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public void OnGet()
    {
        Banner = new QuangCaoBanner { MaQuanLyCapNhat = HttpContext.Session.GetString("UserName") ?? "" };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _db.QuangCaoBanner.Add(Banner);
        await _db.SaveChangesAsync();
        await _log.LogAsync("Admin Thêm banner", $"Liên kết: {Banner.DuongDanLienKet}");
        return RedirectToPage("Index");
    }
}
