using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public List<TaiKhoan> Users { get; set; } = new();

    public IndexModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task OnGetAsync()
    {
        Users = await _db.TaiKhoan.OrderBy(u => u.MaTaiKhoan).ToListAsync();
        await _log.LogAsync("Xem danh sách người dùng");
    }

    public async Task<IActionResult> OnPostUpdateRoleAsync(int maTaiKhoan, string vaiTroMoi)
    {
        var user = await _db.TaiKhoan.FindAsync(maTaiKhoan);
        if (user == null) return NotFound();

        var oldRole = user.VaiTro;
        user.VaiTro = vaiTroMoi;
        await _db.SaveChangesAsync();

        await _log.LogAsync("Đổi quyền người dùng", $"\"{user.TenDangNhap}\": {oldRole} -> {vaiTroMoi}");
        return RedirectToPage();
    }
}
