using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;
using Microsoft.Extensions.Options;

namespace CarProject.Pages;

public class TestDriveModel : PageModel
{
    private const decimal BookingFee = 1_000_000m;

    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    private readonly SepaySettings _sepay;

    public TestDriveModel(AppDbContext db, IActivityLogService log, IOptions<SepaySettings> sepay)
    {
        _db = db;
        _log = log;
        _sepay = sepay.Value;
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
    public string MaChiNhanh { get; set; } = "";

    [BindProperty]
    public DateTime NgayHen { get; set; } = DateTime.Today.AddDays(3);

    [BindProperty]
    public string GioHen { get; set; } = "09:00";

    [BindProperty]
    public string? SoBangLaiXe { get; set; }

    [BindProperty]
    public string? PhuongThucThanhToan { get; set; }

    [BindProperty]
    public string? GhiChu { get; set; }

    [BindProperty(Name = "_ajax")]
    public bool IsAjaxSubmit { get; set; }

    public List<DongXe> DanhSachXe { get; set; } = new();
    public List<ChiNhanhShowroom> DanhSachChiNhanh { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string BankName { get; set; } = "";
    public string BankNumber { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string? TransferContent { get; set; }
    public string? QrImageUrl { get; set; }
    public bool ShowQr { get; set; }

    public async Task OnGetAsync()
    {
        DanhSachXe = await _db.DongXe.Include(d => d.HangXe).ToListAsync();
        DanhSachChiNhanh = await _db.ChiNhanhShowroom.ToListAsync();
        BankName = $"{_sepay.BankAccount} ({_sepay.BankName})";
        BankNumber = _sepay.BankNumber;
        AccountName = _sepay.AccountName;
        await _log.LogAsync("Xem trang đăng ký lái thử");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        DanhSachXe = await _db.DongXe.Include(d => d.HangXe).ToListAsync();
        DanhSachChiNhanh = await _db.ChiNhanhShowroom.ToListAsync();

        if (string.IsNullOrWhiteSpace(SoBangLaiXe))
        {
            ErrorMessage = "Vui lòng nhập số bằng lái xe.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(PhuongThucThanhToan))
        {
            ErrorMessage = "Vui lòng chọn phương thức thanh toán.";
            return Page();
        }

        var minDate = DateTime.Today.AddDays(3);
        var maxDate = DateTime.Today.AddMonths(1);
        if (NgayHen < minDate || NgayHen > maxDate)
        {
            ErrorMessage = "Ngày hẹn phải cách ít nhất 3 ngày và không quá 1 tháng.";
            return Page();
        }

        var userId = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userId))
        {
            ErrorMessage = "Vui lòng đăng nhập để đặt lịch lái thử.";
            return Page();
        }

        var maGiaoDich = $"LD{DateTime.Now:yyMMddHHmmss}";
        var note = $"PTTT: {PhuongThucThanhToan}. {GhiChu ?? string.Empty}".Trim();
        if (string.Equals(PhuongThucThanhToan, "Chuyển khoản", StringComparison.OrdinalIgnoreCase))
        {
            note += $" | TX:{maGiaoDich}";
        }

        var lichHen = new LichHenLaiThu
        {
            MaKhachHang = userId,
            MaDong = MaDong,
            MaChiNhanh = MaChiNhanh,
            HoTenNguoiLai = HoTen,
            SoDienThoai = SoDienThoai,
            SoBangLaiXe = SoBangLaiXe,
            NgayHen = NgayHen,
            GioHen = GioHen,
            TrangThai = "Chờ xác nhận",
            YKienKhachHang = note,
            MaGiaoDich = maGiaoDich
        };

        _db.LichHenLaiThu.Add(lichHen);
        await _db.SaveChangesAsync();
        await _log.LogAsync($"Đăng ký lái thử: {HoTen} - {SoDienThoai}");

        BankName = $"{_sepay.BankAccount} ({_sepay.BankName})";
        BankNumber = _sepay.BankNumber;
        AccountName = _sepay.AccountName;

        if (string.Equals(PhuongThucThanhToan, "Chuyển khoản", StringComparison.OrdinalIgnoreCase))
        {
            TransferContent = maGiaoDich;
            QrImageUrl = CarProject.Services.VietQr.BuildDataUri("970422", _sepay.BankNumber, 10000, maGiaoDich);
            ShowQr = true;
            SuccessMessage = "Yêu cầu đặt lái thử đã được ghi nhận. Vui lòng chuyển khoản 10.000₫ theo mã trên và đợi webhook xác nhận để đơn được gửi showroom.";

            if (IsAjaxSubmit)
            {
                return new JsonResult(new
                {
                    success = true,
                    maGiaoDich,
                    bankName = BankName,
                    bankNumber = BankNumber,
                    accountName = AccountName,
                    paymentMethod = PhuongThucThanhToan,
                    qrUrl = QrImageUrl,
                    message = SuccessMessage
                });
            }
        }
        else
        {
            SuccessMessage = "Đặt lịch lái thử thành công! Chúng tôi sẽ liên hệ bạn sớm nhất.";

            if (Request.Form["_ajax"] == "true")
            {
                return new JsonResult(new
                {
                    success = true,
                    message = SuccessMessage
                });
            }
        }

        return Page();
    }
}
