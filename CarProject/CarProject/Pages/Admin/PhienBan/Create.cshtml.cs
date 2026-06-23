using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.PhienBan;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    [BindProperty]
    public PhienBanXe PhienBan { get; set; }

    public SelectList DongList { get; set; }

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGetAsync()
    {
        var dongList = await _db.DongXe.ToListAsync();
        DongList = new SelectList(dongList, "MaDong", "TenDong");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        _db.PhienBanXe.Add(PhienBan);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
