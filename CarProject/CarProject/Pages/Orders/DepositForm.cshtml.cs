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

    public DongXe Dong { get; set; }
    public List<PhienBanOption> PhienBans { get; set; } = new();
    public List<ChiNhanhShowroom> DanhSachChiNhanh { get; set; } = new();
    public string SuccessMessage { get; set; }
    public decimal TotalDeposit { get; set; }
    public int TongXe { get; set; }

    [BindProperty]
    public string HoTen { get; set; } = "";
    [BindProperty]
    public string SoDienThoai { get; set; } = "";
    [BindProperty]
    public string DiaChi { get; set; } = "";
    [BindProperty]
    public string PhuongThucThanhToan { get; set; } = "";
    [BindProperty]
    public string? GhiChu { get; set; }
    [BindProperty]
    public string MaChiNhanh { get; set; } = "";

    [BindProperty]
    public int SoLuongPhienBan { get; set; } = 1;

    [BindProperty]
    public Dictionary<int, int> SoLuongByPhienBan { get; set; } = new();

    [BindProperty(Name = "_ajax")]
    public bool IsAjaxSubmit { get; set; }

    private Dictionary<(int, string), int> _tonKhoLut = new();

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
        await LoadDongAsync(id);
        if (Dong == null)
            return NotFound();

        await LoadShowroomsAsync();
        await _log.LogAsync("Xem form đặt cọc", $"{Dong.TenDong} (ID={id})");
        return Page();
    }

    private async Task LoadDongAsync(int id)
    {
        Dong = await _db.DongXe
            .Include(d => d.HangXe)
            .Include(d => d.PhienBanXes)
            .FirstOrDefaultAsync(d => d.MaDong == id);
        if (Dong == null) return;

        var phienBans = Dong.PhienBanXes.ToList();
        var maPbs = phienBans.Select(p => p.MaPhienBan).ToList();

        // Số xe đã đặt cọc (chỉ đơn ĐÃ THANH TOÁN giữ xe)
        var daDatCoc = await _db.DonDatCocChiTiet
            .Where(c => maPbs.Contains(c.MaPhienBan)
                && c.DonDatCoc != null
                && c.DonDatCoc!.TrangThaiThanhToan == "Đã thanh toán")
            .GroupBy(c => c.MaPhienBan)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.SoLuong));

        PhienBans = phienBans.Select(p =>
        {
            var reserved = daDatCoc.TryGetValue(p.MaPhienBan, out var r) ? r : 0;
            return new PhienBanOption
            {
                MaPhienBan = p.MaPhienBan,
                TenPhienBan = p.TenPhienBan,
                GiaNiemYet = p.GiaNiemYet,
                SoLuongTrongKho = p.SoLuongTrongKho,
                DaDatCoc = reserved,
                ConLai = Math.Max(0, p.SoLuongTrongKho - reserved),
                IsPreOrder = p.SoLuongTrongKho <= 0,
                MauSac = p.MauSac ?? "",
                DongCo = p.DongCo ?? "",
                HopSo = p.HopSo ?? ""
            };
        }).ToList();
    }

    private async Task LoadShowroomsAsync()
    {
        var all = await _db.ChiNhanhShowroom
            .Where(c => c.TrangThai == "Active" || c.TrangThai == "Hoạt động")
            .ToListAsync();

        // Showroom có xe (còn đủ số chưa đặt cọc) cho bất kỳ phiên bản nào của dòng xe
        var maPbs = PhienBans.Select(p => p.MaPhienBan).ToList();
        var tonKho = await _db.TonKhoTheoChiNhanh
            .Where(t => maPbs.Contains(t.MaPhienBan))
            .ToListAsync();

        var daDatCoc = await _db.DonDatCocChiTiet
            .Where(c => maPbs.Contains(c.MaPhienBan)
                && c.DonDatCoc != null
                && c.DonDatCoc!.TrangThaiThanhToan == "Đã thanh toán")
            .ToListAsync();

        var daDatCocLut = daDatCoc
            .GroupBy(c => c.MaPhienBan + "|" + (c.MaChiNhanh ?? ""))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.SoLuong));

        var maCoXe = tonKho
            .Where(t => t.SoLuong - (daDatCocLut.TryGetValue(t.MaPhienBan + "|" + t.MaChiNhanh, out var r) ? r : 0) > 0)
            .Select(t => t.MaChiNhanh)
            .Distinct()
            .ToHashSet();

        var filtered = all.Where(c => maCoXe.Contains(c.MaChiNhanh)).ToList();
        DanhSachChiNhanh = filtered.Any() ? filtered : all;
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await LoadDongAsync(id);
        if (Dong == null)
            return NotFound();

        if (string.IsNullOrEmpty(HoTen) || string.IsNullOrEmpty(SoDienThoai))
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng điền họ tên và số điện thoại.");
            ModelState.AddModelError("", "Vui lòng điền họ tên và số điện thoại.");
            await LoadShowroomsAsync();
            return Page();
        }

        if (string.IsNullOrEmpty(MaChiNhanh))
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng chọn showroom nguồn.");
            ModelState.AddModelError("", "Vui lòng chọn showroom nguồn.");
            await LoadShowroomsAsync();
            return Page();
        }

        // Lọc các phiên bản người dùng chọn (số lượng > 0)
        var selected = SoLuongByPhienBan
            .Where(kv => kv.Key > 0 && kv.Value > 0)
            .ToList();

        if (selected.Count == 0)
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng chọn ít nhất 1 phiên bản xe.");
            ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 phiên bản xe.");
            await LoadShowroomsAsync();
            return Page();
        }

        // Quy tắc: số phiên bản được chọn phải khớp "Số lượng phiên bản" đã khai báo
        if (selected.Count != SoLuongPhienBan)
        {
            if (IsAjaxSubmit) return JsonError($"Bạn đã chọn {selected.Count} phiên bản nhưng yêu cầu {SoLuongPhienBan}. Vui lòng tích chọn đúng số phiên bản.");
            ModelState.AddModelError("", $"Bạn đã chọn {selected.Count} phiên bản nhưng yêu cầu {SoLuongPhienBan}. Vui lòng tích chọn đúng số phiên bản.");
            await LoadShowroomsAsync();
            return Page();
        }

        // Quy tắc: nếu tổng xe >= 2 thì phải có ít nhất 2 phiên bản khác nhau
        var totalXe = selected.Sum(kv => kv.Value);
        if (totalXe >= 2 && selected.Count < 2)
        {
            if (IsAjaxSubmit) return JsonError("Khi đặt từ 2 xe trở lên phải chọn ít nhất 2 phiên bản khác nhau.");
            ModelState.AddModelError("", "Khi đặt từ 2 xe trở lên phải chọn ít nhất 2 phiên bản khác nhau.");
            await LoadShowroomsAsync();
            return Page();
        }

        // Kiểm tra tồn kho còn đủ theo từng phiên bản (số xe CHƯA đặt cọc)
        foreach (var kv in selected)
        {
            var opt = PhienBans.FirstOrDefault(p => p.MaPhienBan == kv.Key);
            if (opt == null)
            {
                if (IsAjaxSubmit) return JsonError($"Phiên bản {kv.Key} không tồn tại.");
                ModelState.AddModelError("", $"Phiên bản {kv.Key} không tồn tại.");
                await LoadShowroomsAsync();
                return Page();
            }
            if (!opt.IsPreOrder && kv.Value > opt.ConLai)
            {
                var msg = $"Phiên bản \"{opt.TenPhienBan}\" chỉ còn {opt.ConLai} xe chưa đặt cọc, không đủ cho {kv.Value} xe.";
                if (IsAjaxSubmit) return JsonError(msg);
                ModelState.AddModelError("", msg);
                await LoadShowroomsAsync();
                return Page();
            }
        }

        TotalDeposit = selected.Sum(kv =>
        {
            var opt = PhienBans.First(p => p.MaPhienBan == kv.Key);
            return DepositCalculator.Compute(opt.GiaNiemYet, opt.IsPreOrder) * kv.Value;
        });
        TongXe = totalXe;
        var hasPreOrder = selected.Any(kv => PhienBans.First(p => p.MaPhienBan == kv.Key).IsPreOrder);

        // ===== Transaction + khoá dòng để chống bán vượt tồn kho =====
        await using var tx = await _db.Database.BeginTransactionAsync();

        foreach (var kv in selected)
        {
            await _db.Database.ExecuteSqlRawAsync(
                "SELECT MaTonKho FROM TonKhoTheoChiNhanh WITH (UPDLOCK, HOLDLOCK) WHERE MaPhienBan = {0}", kv.Key);
        }

        var selectedPbs = selected.Select(kv => kv.Key).ToList();
        var tonKhoRows = await _db.TonKhoTheoChiNhanh
            .Where(t => selectedPbs.Contains(t.MaPhienBan))
            .ToListAsync();
        _tonKhoLut = tonKhoRows
            .GroupBy(t => (t.MaPhienBan, t.MaChiNhanh))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.SoLuong));

        var maGiaoDich = $"MLC{DateTime.Now:yyMMddHHmmss}{Dong.MaDong}";
        var deposit = new DonDatCoc
        {
            MaKhachHang = User.GetJwtUserName() ?? "",
            MaChiNhanh = MaChiNhanh,
            SoTienCoc = TotalDeposit,
            PhuongThucThanhToan = string.IsNullOrEmpty(PhuongThucThanhToan) ? "Chuyển khoản" : PhuongThucThanhToan,
            TrangThaiThanhToan = "Chưa thanh toán",
            TrangThaiDonHang = hasPreOrder ? "Chờ xử lý" : "Chờ xác nhận",
            NgayTaoDon = DateTime.Now,
            HoTen = HoTen,
            SoDienThoai = SoDienThoai,
            DiaChi = DiaChi ?? "",
            GhiChu = GhiChu ?? "",
            MaGiaoDich = maGiaoDich
        };

        foreach (var kv in selected)
        {
            var opt = PhienBans.First(p => p.MaPhienBan == kv.Key);
            var isPreOrder = opt.IsPreOrder;
            var tonKhoCn = _tonKhoLut.TryGetValue((kv.Key, MaChiNhanh), out var tk) ? tk : 0;
            var soLuongThieu = isPreOrder ? kv.Value : Math.Max(0, kv.Value - tonKhoCn);

            deposit.ChiTiets.Add(new DonDatCocChiTiet
            {
                MaPhienBan = kv.Key,
                MaChiNhanh = MaChiNhanh,
                SoLuong = kv.Value,
                SoTienCocPhanBo = DepositCalculator.Compute(opt.GiaNiemYet, isPreOrder) * kv.Value,
                SoLuongThieu = soLuongThieu
            });
        }

        _db.DonDatCoc.Add(deposit);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        var listXe = string.Join(", ", selected.Select(kv =>
        {
            var opt = PhienBans.First(p => p.MaPhienBan == kv.Key);
            return $"{opt.TenPhienBan} x{kv.Value}";
        }));

        await _log.LogAsync("Gửi đơn đặt cọc",
            $"{HoTen} - {SoDienThoai} - {Dong.TenDong} ({listXe}) - {TotalDeposit:N0} VNĐ");

        var result = new
        {
            maDonCoc = deposit.MaDonCoc,
            maGiaoDich = deposit.MaGiaoDich,
            soTienCoc = TotalDeposit,
            bankName = $"{_sepay.BankAccount} ({_sepay.BankName})",
            bankNumber = _sepay.BankNumber,
            accountName = _sepay.AccountName,
            transferContent = maGiaoDich,
            tenPhienBan = Dong.TenDong,
            hoTen = HoTen,
            soDienThoai = SoDienThoai,
            dongCo = "",
            hopSo = "",
            mauSac = "",
            loaiNhietLieu = "",
            giaNiemYet = 0,
            soLuongXe = totalXe
        };

        TempData["DepositResult"] = JsonSerializer.Serialize(result);

        if (IsAjaxSubmit)
        {
            var bin = "970422";
            var qrUrl = CarProject.Services.VietQr.BuildDataUri(bin, _sepay.BankNumber, 10000, maGiaoDich);
            var tongTienGoc = selected.Sum(kv => (long)PhienBans.First(p => p.MaPhienBan == kv.Key).GiaNiemYet * kv.Value);
            var ajaxRateList = selected.Select(kv => PhienBans.First(p => p.MaPhienBan == kv.Key).IsPreOrder ? "15%" : "20%").Distinct().ToList();
            var ajaxRateText = ajaxRateList.Count == 1 ? ajaxRateList[0] : "15%/20%";
            return new JsonResult(new
            {
                success = true,
                maDonCoc = deposit.MaDonCoc,
                maGiaoDich = deposit.MaGiaoDich,
                soTienCoc = TotalDeposit,
                tongTienGoc,
                cocRateText = ajaxRateText,
                bankName = $"{_sepay.BankAccount} ({_sepay.BankName})",
                bankNumber = _sepay.BankNumber,
                accountName = _sepay.AccountName,
                qrUrl
            });
        }

        return RedirectToPage("/Orders/Payment", new { maDonCoc = deposit.MaDonCoc });
    }

    public class PhienBanOption
    {
        public int MaPhienBan { get; set; }
        public string TenPhienBan { get; set; } = "";
        public long GiaNiemYet { get; set; }
        public int SoLuongTrongKho { get; set; }
        public int DaDatCoc { get; set; }
        public int ConLai { get; set; }
        public bool IsPreOrder { get; set; }
        public string MauSac { get; set; } = "";
        public string DongCo { get; set; } = "";
        public string HopSo { get; set; } = "";
    }

    private IActionResult JsonError(string message)
        => new JsonResult(new { success = false, error = message });
}
