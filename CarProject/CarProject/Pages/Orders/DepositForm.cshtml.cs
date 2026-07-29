using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarProject.Pages.Orders;

[Authorize]
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

    [BindProperty(Name = "_ajax")]
    public bool IsAjaxSubmit { get; set; }

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

        // Kiểm tra tồn kho
        var reservedStatuses = new[] { "Chờ xử lý", "Chờ xác nhận", "Đã xác nhận", "Đã thanh toán", "Hoàn tất" };
        var reservedQty = await _db.DonDatCoc
            .CountAsync(d => d.MaPhienBan == DepositData.MaPhienBan
                && reservedStatuses.Contains(d.TrangThaiDonHang ?? ""));
        var available = PhienBan.SoLuongTrongKho - reservedQty;
        if (available <= 0)
        {
            ModelState.AddModelError("", "Xe này đã hết hàng. Vui lòng chọn xe khác.");
            return Page();
        }

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
            hoTen = DepositData.HoTen,
            soDienThoai = DepositData.SoDienThoai ?? "",
            dongCo = PhienBan?.DongCo ?? "",
            hopSo = PhienBan?.HopSo ?? "",
            mauSac = PhienBan?.MauSac ?? "",
            loaiNhietLieu = PhienBan?.LoaiNhietLieu ?? "",
            giaNiemYet = PhienBan?.GiaNiemYet ?? 0
        };

        TempData["DepositResult"] = JsonSerializer.Serialize(result);

        if (IsAjaxSubmit)
        {
            var bin = "970436";
            var qrUrl = $"https://img.vietqr.io/image/{bin}-{_sepay.BankNumber}-compact2.jpg?amount=10000&addInfo={Uri.EscapeDataString(deposit.MaGiaoDich ?? "")}&accountName={Uri.EscapeDataString(_sepay.AccountName)}";
            return new JsonResult(new
            {
                success = true,
                maDonCoc = deposit.MaDonCoc,
                maGiaoDich = deposit.MaGiaoDich,
                soTienCoc = deposit.SoTienCoc,
                bankName = $"{_sepay.BankAccount} ({_sepay.BankName})",
                bankNumber = _sepay.BankNumber,
                accountName = _sepay.AccountName,
                qrUrl
            });
        }

        return RedirectToPage("/Orders/Payment", new { maDonCoc = deposit.MaDonCoc });
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
