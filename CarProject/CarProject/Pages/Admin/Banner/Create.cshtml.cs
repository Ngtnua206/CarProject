using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.Banner;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public QuangCaoBanner Banner { get; set; }

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    public void OnGet()
    {
        Banner = new QuangCaoBanner { MaQuanLyCapNhat = 1 };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _db.QuangCaoBanner.Add(Banner);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
