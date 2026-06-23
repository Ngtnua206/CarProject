using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.HangXe;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public Models.HangXe HangXe { get; set; }

    public EditModel(AppDbContext db)
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
        if (!ModelState.IsValid)
            return Page();

        _db.HangXe.Update(HangXe);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
