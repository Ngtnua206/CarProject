using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.DonCoc;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    [BindProperty]
    public DonDatCoc DonCoc { get; set; }

    public SelectList KhachHangList { get; set; }
    public SelectList PhienBanList { get; set; }

    public EditModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        DonCoc = await _db.DonDatCoc.FindAsync(id);
        if (DonCoc == null)
            return NotFound();

        KhachHangList = new SelectList(
            await _db.TaiKhoan.ToListAsync(),
            "TenDangNhap", "TenDangNhap", DonCoc.MaKhachHang);
        PhienBanList = new SelectList(
            await _db.PhienBanXe.ToListAsync(),
            "MaPhienBan", "TenPhienBan", DonCoc.MaPhienBan);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var existing = await _db.DonDatCoc.FindAsync(DonCoc.MaDonCoc);
        if (existing == null)
            return NotFound();

        existing.MaKhachHang = DonCoc.MaKhachHang;
        existing.MaPhienBan = DonCoc.MaPhienBan;
        existing.SoTienCoc = DonCoc.SoTienCoc;
        existing.PhuongThucThanhToan = DonCoc.PhuongThucThanhToan;
        existing.TrangThaiThanhToan = DonCoc.TrangThaiThanhToan;
        existing.NgayTaoDon = DonCoc.NgayTaoDon;
        existing.NgayHenNhanXe = DonCoc.NgayHenNhanXe;
        existing.TrangThaiDonHang = DonCoc.TrangThaiDonHang;
        existing.GhiChu = DonCoc.GhiChu;

        await _db.SaveChangesAsync();
        await _log.LogAsync("Admin Sửa đơn cọc", $"Mã đơn={DonCoc.MaDonCoc}, tiền={DonCoc.SoTienCoc:N0}");
        return RedirectToPage("Index");
    }
}
