using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.Orders;

public class DepositFormModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;

    public PhienBanXe PhienBan { get; set; }
    public string SuccessMessage { get; set; }

    [BindProperty]
    public DepositRequest DepositData { get; set; }

    public DepositFormModel(AppDbContext db, IActivityLogService log)
    {
        _db = db;
        _log = log;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        PhienBan = await _db.PhienBanXe
            .Include(p => p.DongXe)
            .FirstOrDefaultAsync(p => p.MaPhienBan == id);

        if (PhienBan == null)
            return NotFound();

        await _log.LogAsync("Xem form đặt cọc", $"{PhienBan.TenPhienBan} (ID={id})");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            PhienBan = await _db.PhienBanXe
                .Include(p => p.DongXe)
                .FirstOrDefaultAsync(p => p.MaPhienBan == DepositData.MaPhienBan);
            return Page();
        }

        // Demo: không lưu vào database, chỉ hiển thị thành công
        SuccessMessage = $"Cảm ơn {DepositData.HoTen}! Đơn đặt cọc của bạn đã được tiếp nhận. " +
                        $"Chúng tôi sẽ liên hệ với bạn qua số {DepositData.SoDienThoai} sớm nhất.";

        PhienBan = await _db.PhienBanXe
            .Include(p => p.DongXe)
            .FirstOrDefaultAsync(p => p.MaPhienBan == DepositData.MaPhienBan);

        await _log.LogAsync("Gửi đơn đặt cọc",
            $"{DepositData.HoTen} - {DepositData.SoDienThoai} - {PhienBan?.TenPhienBan} - {DepositData.SoTienCoc:N0} VNĐ");

        return Page();
    }

    public class DepositRequest
    {
        public int MaPhienBan { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public decimal SoTienCoc { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string GhiChu { get; set; }
    }
}
