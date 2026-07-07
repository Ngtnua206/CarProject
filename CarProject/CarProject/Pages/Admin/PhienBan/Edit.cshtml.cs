using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.PhienBan;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public PhienBanXe PhienBan { get; set; }

    public SelectList DongList { get; set; }

    public EditModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        PhienBan = await _db.PhienBanXe.FindAsync(id);
        if (PhienBan == null)
            return NotFound();

        var dongList = await _db.DongXe.ToListAsync();
        DongList = new SelectList(dongList, "MaDong", "TenDong", PhienBan.MaDong);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var dongList = await _db.DongXe.ToListAsync();
            DongList = new SelectList(dongList, "MaDong", "TenDong", PhienBan.MaDong);
            return Page();
        }

        var existing = await _db.PhienBanXe.FindAsync(PhienBan.MaPhienBan);
        if (existing == null)
            return NotFound();

        existing.MaDong = PhienBan.MaDong;
        existing.TenPhienBan = PhienBan.TenPhienBan;
        existing.GiaNiemYet = PhienBan.GiaNiemYet;
        existing.MauSac = PhienBan.MauSac;
        existing.DongCo = PhienBan.DongCo;
        existing.HopSo = PhienBan.HopSo;
        existing.LoaiNhietLieu = PhienBan.LoaiNhietLieu;
        existing.SoLuongTrongKho = PhienBan.SoLuongTrongKho;
        existing.DuongDanAnh = PhienBan.DuongDanAnh;
        existing.TrangThai = PhienBan.TrangThai;

        await _db.SaveChangesAsync();
        await _log.LogAsync("Admin Sửa phiên bản", $"{existing.TenPhienBan} (ID={existing.MaPhienBan})");
        return RedirectToPage("Index");
    }
}
