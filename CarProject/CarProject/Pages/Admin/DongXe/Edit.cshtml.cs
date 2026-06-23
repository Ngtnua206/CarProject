using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.DongXe;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public Models.DongXe DongXe { get; set; }

    public SelectList HangList { get; set; }

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        DongXe = await _db.DongXe.FindAsync(id);
        if (DongXe == null)
            return NotFound();

        var hangList = await _db.HangXe.ToListAsync();
        HangList = new SelectList(hangList, "MaHang", "TenHang", DongXe.MaHang);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var hangList = await _db.HangXe.ToListAsync();
            HangList = new SelectList(hangList, "MaHang", "TenHang", DongXe.MaHang);
            return Page();
        }

        _db.DongXe.Update(DongXe);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
