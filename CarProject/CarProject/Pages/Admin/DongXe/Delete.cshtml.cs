using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.DongXe;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public Models.DongXe DongXe { get; set; }

    public DeleteModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        DongXe = await _db.DongXe.FindAsync(id);
        if (DongXe == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (DongXe == null)
            return NotFound();

        var name = DongXe.TenDong;
        _db.DongXe.Remove(DongXe);
        await _db.SaveChangesAsync();
        await _log.LogAsync("Admin Xóa dòng xe", $"{name} (ID={DongXe.MaDong})");
        return RedirectToPage("Index");
    }
}
