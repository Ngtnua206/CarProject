using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.HangXe;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public Models.HangXe HangXe { get; set; }

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        HangXe = await _db.HangXe.FindAsync(id);
        if (HangXe == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HangXe == null)
            return NotFound();

        _db.HangXe.Remove(HangXe);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
