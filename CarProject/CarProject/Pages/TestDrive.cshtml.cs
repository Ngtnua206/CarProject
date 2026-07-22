using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages;

public class TestDriveModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public TestDriveModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    [BindProperty]
    public string HoTen { get; set; } = "";

    [BindProperty]
    public string SoDienThoai { get; set; } = "";

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public int MaDong { get; set; }

    [BindProperty]
    public DateTime NgayHen { get; set; } = DateTime.Today.AddDays(1);

    [BindProperty]
    public string GioHen { get; set; } = "09:00";

    [BindProperty]
    public string? GhiChu { get; set; }

    public List<DongXe> DanhSachXe { get; set; } = new();
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        DanhSachXe = await _db.DongXe.Include(d => d.HangXe).ToListAsync();
        await _log.LogAsync("Xem trang đăng ký lái thử");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        DanhSachXe = await _db.DongXe.Include(d => d.HangXe).ToListAsync();

        var userId = User.GetJwtUserName();
        var lichHen = new LichHenLaiThu
        {
            MaKhachHang = userId ?? "",
            MaDong = MaDong,
            HoTenNguoiLai = HoTen,
            SoDienThoai = SoDienThoai,
            NgayHen = NgayHen,
            GioHen = GioHen,
            TrangThai = "Chờ xác nhận",
            YKienKhachHang = GhiChu ?? ""
        };

        _db.LichHenLaiThu.Add(lichHen);
        await _db.SaveChangesAsync();
        await _log.LogAsync($"Đăng ký lái thử: {HoTen} - {SoDienThoai}");

        SuccessMessage = "Đặt lịch lái thử thành công! Chúng tôi sẽ liên hệ bạn sớm nhất.";
        return Page();
    }
}
