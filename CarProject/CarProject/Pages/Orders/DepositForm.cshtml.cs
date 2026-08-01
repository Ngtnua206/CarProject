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
    public List<ChiNhanhShowroom> DanhSachChiNhanh { get; set; } = new();
    public bool IsPreOrder { get; set; }

    [BindProperty]
    public DepositRequest DepositData { get; set; }

    [BindProperty]
    public string MaChiNhanh { get; set; } = "";

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

        IsPreOrder = PhienBan.SoLuongTrongKho <= 0;

        if (!IsPreOrder)
        {
            TempData["DepositError"] = "Xe đang còn hàng trong kho, không cần đặt cọc trước. Hãy thêm vào giỏ hàng để đặt mua.";
            return RedirectToPage("/Details", new { id = PhienBan.MaDong });
        }

        await LoadShowrooms();
        await _log.LogAsync("Xem form đặt cọc", $"{PhienBan.TenPhienBan} (ID={id})");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng kiểm tra lại thông tin đặt cọc.");
            PhienBan = await _db.PhienBanXe
                .Include(p => p.DongXe)
                .FirstOrDefaultAsync(p => p.MaPhienBan == DepositData.MaPhienBan);
            IsPreOrder = PhienBan?.SoLuongTrongKho <= 0;
            await LoadShowrooms();
            return Page();
        }

        if (string.IsNullOrEmpty(DepositData.HoTen) || string.IsNullOrEmpty(DepositData.SoDienThoai))
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng điền họ tên và số điện thoại.");
            PhienBan = await _db.PhienBanXe
                .Include(p => p.DongXe)
                .FirstOrDefaultAsync(p => p.MaPhienBan == DepositData.MaPhienBan);
            IsPreOrder = PhienBan?.SoLuongTrongKho <= 0;
            await LoadShowrooms();
            ModelState.AddModelError("", "Vui lòng điền họ tên và số điện thoại.");
            return Page();
        }

        PhienBan = await _db.PhienBanXe
            .Include(p => p.DongXe)
            .FirstOrDefaultAsync(p => p.MaPhienBan == DepositData.MaPhienBan);

        if (PhienBan == null)
            return NotFound();

        IsPreOrder = PhienBan.SoLuongTrongKho <= 0;

        if (string.IsNullOrEmpty(MaChiNhanh))
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng chọn showroom nguồn (nơi chuẩn bị xe).");
            ModelState.AddModelError("", "Vui lòng chọn showroom nguồn (nơi chuẩn bị xe).");
            await LoadShowrooms();
            return Page();
        }

        // Đặt cọc 1 xe chỉ hiệu nghiệm khi xe hết hàng (đặt trước)
        if (!IsPreOrder)
        {
            var msg = "Xe đang còn hàng trong kho, không cần đặt cọc trước. Hãy thêm vào giỏ hàng để đặt mua.";
            if (IsAjaxSubmit) return JsonError(msg);
            ModelState.AddModelError("", msg);
            await LoadShowrooms();
            return Page();
        }

        // ===== Transaction + khoá dòng để chống bán vượt tồn kho =====
        await using var tx = await _db.Database.BeginTransactionAsync();

        // Khoá các dòng tồn kho của phiên bản này
        await _db.Database.ExecuteSqlRawAsync(
            "SELECT MaTonKho FROM TonKhoTheoChiNhanh WITH (UPDLOCK, HOLDLOCK) WHERE MaPhienBan = {0}", PhienBan.MaPhienBan);

        var deposit = new DonDatCoc
        {
            MaKhachHang = User.GetJwtUserName() ?? "",
            MaPhienBan = DepositData.MaPhienBan,
            MaChiNhanh = MaChiNhanh,
            SoTienCoc = DepositData.SoTienCoc,
            PhuongThucThanhToan = DepositData.PhuongThucThanhToan ?? "Chuyển khoản",
            TrangThaiThanhToan = "Chưa thanh toán",
            TrangThaiDonHang = IsPreOrder ? "Chờ xử lý" : "Chờ xác nhận",
            NgayTaoDon = DateTime.Now,
            HoTen = DepositData.HoTen,
            SoDienThoai = DepositData.SoDienThoai,
            DiaChi = DepositData.DiaChi ?? "",
            GhiChu = DepositData.GhiChu ?? "",
            MaGiaoDich = $"MLC{DateTime.Now:yyMMddHHmmss}-{DepositData.MaPhienBan}"
        };

        deposit.ChiTiets.Add(new DonDatCocChiTiet
        {
            MaPhienBan = DepositData.MaPhienBan,
            MaChiNhanh = MaChiNhanh,
            SoLuong = 1
        });

        _db.DonDatCoc.Add(deposit);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var tenXe = $"{PhienBan.DongXe?.TenDong ?? ""} {PhienBan.TenPhienBan}".Trim();

        await _log.LogAsync("Gửi đơn đặt cọc",
            $"{DepositData.HoTen} - {DepositData.SoDienThoai} - {tenXe} - {DepositData.SoTienCoc:N0} VNĐ");

        await _notif.SendToRoleAsync("Admin", "Đơn đặt cọc mới",
            $"Đơn cọc #{deposit.MaDonCoc} - {DepositData.HoTen} - {tenXe} - {DepositData.SoTienCoc:N0}đ",
            $"/Admin/DonCoc/Edit?maDonCoc={deposit.MaDonCoc}");

        // Gửi thông báo cho quản lý showroom nguồn (đặt trước: cần chuẩn bị xe)
        var cn = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaChiNhanh == MaChiNhanh);
        if (cn?.MaQuanLy != null)
        {
            var noiDung = IsPreOrder
                ? $"Khách {DepositData.HoTen} muốn mua:\n- 1 xe {tenXe}\nĐịa điểm hẹn: {cn.TenChiNhanh}\n\nXe hết hàng - bạn vui lòng Chấp nhận để chuẩn bị xe hoặc Từ chối kèm lý do."
                : $"Khách {DepositData.HoTen} muốn mua:\n- 1 xe {tenXe}\nĐịa điểm hẹn: {cn.TenChiNhanh}\n\nVui lòng chọn Tiếp nhận hoặc Không tiếp nhận.";
            await _notif.SendAsync(cn.MaQuanLy, "Đơn đặt cọc mới - cần xác nhận",
                $"{noiDung} (Đơn cọc #{deposit.MaDonCoc})",
                $"/QuanLy/DonCoc?highlight={deposit.MaDonCoc}");
        }

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
        public string? GhiChu { get; set; }
    }

    private async Task LoadShowrooms()
    {
        DanhSachChiNhanh = await _db.ChiNhanhShowroom
            .Where(c => c.TrangThai == "Active" || c.TrangThai == "Hoạt động")
            .ToListAsync();
    }

    private IActionResult JsonError(string message)
        => new JsonResult(new { success = false, error = message });
}
