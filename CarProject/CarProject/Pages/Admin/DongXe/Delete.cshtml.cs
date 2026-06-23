using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.DongXe;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public Models.DongXe DongXe { get; set; }

    public DeleteModel(AppDbContext db)
    {
        _db = db;
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

        _db.DongXe.Remove(DongXe);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
