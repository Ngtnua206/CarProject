using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarProject.Pages.Orders;

public class DepositFormModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    private readonly INotificationService _notif;
    private readonly SepaySettings _sepay;

    public PhienBanXe PhienBan { get; set; }
    public string SuccessMessage { get; set; }

    [BindProperty]
    public DepositRequest DepositData { get; set; }

    public DepositFormModel(AppDbContext db, IActivityLogService log, INotificationService notif,
        IOptions<SepaySettings> sepay)
    {
        _db = db;
        _log = log;
        _notif = notif;
        _sepay = sepay.Value;
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

        if (string.IsNullOrEmpty(DepositData.HoTen) || string.IsNullOrEmpty(DepositData.SoDienThoai))
        {
            PhienBan = await _db.PhienBanXe
                .Include(p => p.DongXe)
                .FirstOrDefaultAsync(p => p.MaPhienBan == DepositData.MaPhienBan);
            ModelState.AddModelError("", "Vui lòng điền họ tên và số điện thoại.");
            return Page();
        }

        PhienBan = await _db.PhienBanXe
            .Include(p => p.DongXe)
            .FirstOrDefaultAsync(p => p.MaPhienBan == DepositData.MaPhienBan);

        if (PhienBan == null)
            return NotFound();

        var deposit = new DonDatCoc
        {
            MaKhachHang = User.GetJwtUserName() ?? "",
            MaPhienBan = DepositData.MaPhienBan,
            SoTienCoc = DepositData.SoTienCoc,
            PhuongThucThanhToan = DepositData.PhuongThucThanhToan ?? "Chuyển khoản",
            TrangThaiThanhToan = "Chưa thanh toán",
            TrangThaiDonHang = "Chờ xử lý",
            NgayTaoDon = DateTime.Now,
            HoTen = DepositData.HoTen,
            SoDienThoai = DepositData.SoDienThoai,
            DiaChi = DepositData.DiaChi ?? "",
            GhiChu = DepositData.GhiChu ?? "",
            MaGiaoDich = $"MLC{DateTime.Now:yyMMddHHmmss}-{DepositData.MaPhienBan}"
        };

        _db.DonDatCoc.Add(deposit);
        await _db.SaveChangesAsync();

        var tenXe = $"{PhienBan.DongXe?.TenDong ?? ""} {PhienBan.TenPhienBan}".Trim();

        await _notif.SendAsync(deposit.MaKhachHang, "Đặt cọc thành công",
            $"Xe {tenXe} - {DepositData.SoTienCoc:N0} VNĐ. Mã đơn: #{deposit.MaDonCoc}",
            $"/Orders/DepositResult?maDonCoc={deposit.MaDonCoc}");

        await _notif.SendToRoleAsync("Admin", "Đơn cọc mới",
            $"{DepositData.HoTen} đặt cọc {tenXe} - {DepositData.SoTienCoc:N0} VNĐ",
            $"/Admin/DonCoc/Edit?maDonCoc={deposit.MaDonCoc}");

        var showroom = await _db.ChiNhanhShowroom
            .Where(c => c.MaQuanLy != null && c.TrangThai == "Active")
            .FirstOrDefaultAsync();

        if (showroom != null)
        {
            await _notif.SendAsync(showroom.MaQuanLy, "Đơn cọc mới",
                $"{DepositData.HoTen} đặt cọc {tenXe} - {DepositData.SoTienCoc:N0} VNĐ",
                $"/QuanLy/Dashboard");
        }

        await _log.LogAsync("Gửi đơn đặt cọc",
            $"{DepositData.HoTen} - {DepositData.SoDienThoai} - {tenXe} - {DepositData.SoTienCoc:N0} VNĐ");

        // Chuyển đến trang kết quả với thông tin chuyển khoản VA
        var result = new
        {
            maDonCoc = deposit.MaDonCoc,
            maGiaoDich = deposit.MaGiaoDich,
            soTienCoc = deposit.SoTienCoc,
            bankName = $"{_sepay.BankAccount} ({_sepay.BankName})",
            bankNumber = _sepay.BankNumber,
            accountName = _sepay.AccountName,
            transferContent = deposit.MaGiaoDich,
            tenPhienBan = tenXe,
            hoTen = DepositData.HoTen
        };

        TempData["DepositResult"] = JsonSerializer.Serialize(result);
        return RedirectToPage("/Orders/DepositResult");
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
