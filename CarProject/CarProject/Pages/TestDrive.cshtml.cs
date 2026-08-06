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
    private readonly CarProject.Services.INotificationService _notification;

    public TestDriveModel(AppDbContext db, IActivityLogService log, IOptions<SepaySettings> sepay, CarProject.Services.INotificationService notification)
    {
        _db = db;
        _log = log;
        _sepay = sepay.Value;
        _notification = notification;
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

        var isAjaxSubmit = IsAjaxSubmit || string.Equals(Request.Form["_ajax"], "true", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(HoTen))
        {
            ErrorMessage = "Vui lòng nhập họ và tên.";
            return isAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (string.IsNullOrWhiteSpace(SoDienThoai))
        {
            ErrorMessage = "Vui lòng nhập số điện thoại.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (string.IsNullOrWhiteSpace(SoBangLaiXe))
        {
            ErrorMessage = "Vui lòng nhập số bằng lái xe.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (string.IsNullOrWhiteSpace(PhuongThucThanhToan))
        {
            ErrorMessage = "Vui lòng chọn phương thức thanh toán.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (MaDong <= 0)
        {
            ErrorMessage = "Vui lòng chọn dòng xe.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (string.IsNullOrWhiteSpace(MaChiNhanh))
        {
            ErrorMessage = "Vui lòng chọn showroom.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (!DanhSachXe.Any(d => d.MaDong == MaDong))
        {
            ErrorMessage = "Dòng xe đã chọn không hợp lệ.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (!DanhSachChiNhanh.Any(c => c.MaChiNhanh == MaChiNhanh))
        {
            ErrorMessage = "Showroom đã chọn không hợp lệ.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        var minDate = DateTime.Today.AddDays(3);
        var maxDate = DateTime.Today.AddMonths(1);
        if (NgayHen < minDate || NgayHen > maxDate)
        {
            ErrorMessage = "Ngày hẹn phải cách ít nhất 3 ngày và không quá 1 tháng.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        var userId = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userId))
        {
            ErrorMessage = "Vui lòng đăng nhập để đặt lịch lái thử.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        if (!await _db.TaiKhoan.AnyAsync(t => t.TenDangNhap == userId))
        {
            ErrorMessage = "Tài khoản không hợp lệ. Vui lòng đăng nhập lại.";
            return IsAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        var maGiaoDich = $"LD{DateTime.Now:yyMMddHHmmss}";
        var note = $"PTTT: {PhuongThucThanhToan}. {GhiChu ?? string.Empty}".Trim();
        if (string.Equals(PhuongThucThanhToan, "Chuyển khoản", StringComparison.OrdinalIgnoreCase))
        {
            note += $" | TX:{maGiaoDich}";
        }

        // Defensive checks: ensure referenced FK entities exist to avoid DbUpdateException
        var dong = await _db.DongXe.FindAsync(MaDong);
        if (dong == null)
        {
            ErrorMessage = "Dòng xe đã chọn không tồn tại trong hệ thống.";
            return isAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        var showroom = await _db.ChiNhanhShowroom.FindAsync(MaChiNhanh);
        if (showroom == null)
        {
            ErrorMessage = "Showroom đã chọn không tồn tại trong hệ thống.";
            return isAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
        }

        var taiKhoan = await _db.TaiKhoan.FirstOrDefaultAsync(t => t.TenDangNhap == userId);
        if (taiKhoan == null)
        {
            ErrorMessage = "Tài khoản không tồn tại trong hệ thống. Vui lòng đăng nhập lại.";
            return isAjaxSubmit ? new JsonResult(new { success = false, error = ErrorMessage }) : Page();
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
            MaGiaoDich = maGiaoDich,
            DongXe = dong,
            ChiNhanh = showroom,
            KhachHang = taiKhoan
        };

        try
        {
            _db.LichHenLaiThu.Add(lichHen);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            // Log full exception for diagnostics
            await _log.LogAsync($"Lỗi khi lưu LichHenLaiThu: {innerMessage} | Exception: {ex}");
            ErrorMessage = "Không thể lưu lịch hẹn lái thử do dữ liệu không hợp lệ. Vui lòng thử lại.";
            if (isAjaxSubmit)
            {
                return new JsonResult(new { success = false, error = ErrorMessage, detail = innerMessage });
            }

            ModelState.AddModelError(string.Empty, innerMessage);
            return Page();
        }
        await _log.LogAsync($"Đăng ký lái thử: {HoTen} - {SoDienThoai}");

        // Notify showroom manager (if assigned)
        try
        {
            var manager = showroom?.MaQuanLy;
            if (!string.IsNullOrEmpty(manager))
            {
                var title = "Yêu cầu lái thử mới";
                var content = $"Khách: {HoTen} ({SoDienThoai}) đã đặt lái thử {dong.TenDong} vào {NgayHen:yyyy-MM-dd} {GioHen}. Vui lòng kiểm tra và chấp nhận.";
                var link = $"/QuanLy/LichHen?highlight={lichHen.MaLichHen}";
                await _notification.SendAsync(manager, title, content, link);
            }
            // Also create a user notification
            var userNotifTitle = "Đơn đặt lái thử đã được ghi nhận";
            var userContent = SuccessMessage ?? "Chúng tôi đã ghi nhận yêu cầu lái thử của bạn.";
            await _notification.SendAsync(taiKhoan.TenDangNhap, userNotifTitle, userContent, "/Profile");
        }
        catch
        {
            // Do not block main flow on notification errors
        }

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
