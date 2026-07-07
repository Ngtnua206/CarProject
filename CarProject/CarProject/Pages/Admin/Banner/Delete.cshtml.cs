using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.Banner;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public QuangCaoBanner Banner { get; set; }

    public DeleteModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Banner = await _db.QuangCaoBanner.FindAsync(id);
        if (Banner == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Banner == null)
            return NotFound();

        var ma = Banner.MaBanner;
        _db.QuangCaoBanner.Remove(Banner);
        await _db.SaveChangesAsync();
        await _log.LogAsync("Admin Xóa banner", $"Mã banner={ma}");
        return RedirectToPage("Index");
    }
}
