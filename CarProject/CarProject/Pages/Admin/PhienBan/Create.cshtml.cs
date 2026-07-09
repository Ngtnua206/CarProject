using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.PhienBan;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public PhienBanXe PhienBan { get; set; }

    public SelectList DongList { get; set; }
    public SelectList KhuyenMaiList { get; set; }

    public CreateModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task OnGetAsync()
    {
        var dongList = await _db.DongXe.ToListAsync();
        DongList = new SelectList(dongList, "MaDong", "TenDong");
        var kmList = await _db.ChuongTrinhKhuyenMai.ToListAsync();
        KhuyenMaiList = new SelectList(kmList, "MaKhuyenMai", "TieuDe");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        PhienBan.TrangThai = string.IsNullOrEmpty(PhienBan.TrangThai) ? "Còn hàng" : PhienBan.TrangThai;
        PhienBan.MaKhuyenMai = string.IsNullOrEmpty(PhienBan.MaKhuyenMai) ? "KM00" : PhienBan.MaKhuyenMai;
        PhienBan.DuongDanAnh = string.IsNullOrEmpty(PhienBan.DuongDanAnh) ? "/images/cars/default.jpg" : PhienBan.DuongDanAnh;

        if (!ModelState.IsValid)
        {
            var dongList = await _db.DongXe.ToListAsync();
            DongList = new SelectList(dongList, "MaDong", "TenDong");
            var kmList = await _db.ChuongTrinhKhuyenMai.ToListAsync();
            KhuyenMaiList = new SelectList(kmList, "MaKhuyenMai", "TieuDe");
            return Page();
        }

        _db.PhienBanXe.Add(PhienBan);
        await _db.SaveChangesAsync();
        await _log.LogAsync("Admin Thêm phiên bản", $"{PhienBan.TenPhienBan} - {PhienBan.GiaNiemYet:N0} VNĐ");
        TempData["Success"] = $"Đã thêm phiên bản \"{PhienBan.TenPhienBan}\" thành công.";
        return RedirectToPage("Index");
    }
}
