using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.HangXe;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public Models.HangXe HangXe { get; set; }

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _db.HangXe.Add(HangXe);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
