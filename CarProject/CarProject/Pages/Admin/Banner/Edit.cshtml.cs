using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.Banner;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public QuangCaoBanner Banner { get; set; }

    public EditModel(AppDbContext db, IActivityLogService log)
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
        if (!ModelState.IsValid)
            return Page();

        var existing = await _db.QuangCaoBanner.FindAsync(Banner.MaBanner);
        if (existing == null)
            return NotFound();

        existing.DuongDanAnh = Banner.DuongDanAnh;
        existing.DuongDanLienKet = Banner.DuongDanLienKet;
        existing.ThuTuHienThi = Banner.ThuTuHienThi;
        existing.TrangThaiKichHoat = Banner.TrangThaiKichHoat;

        await _db.SaveChangesAsync();
        await _log.LogAsync("Admin Sửa banner", $"Liên kết: {Banner.DuongDanLienKet}");
        return RedirectToPage("Index");
    }
}
