using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;
using System.Text.Json;

namespace CarProject.Pages.Orders.Cart;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly ICartService _cart;
    private readonly IActivityLogService _log;
    private readonly INotificationService _notif;

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
    public Dictionary<int, int> QuantityInput { get; set; } = new();

    [BindProperty]
    public Dictionary<int, string> SourceChiNhanh { get; set; } = new();

    [BindProperty]
    public string? LichHenNgay { get; set; }
    [BindProperty]
    public string? LichHenGio { get; set; }

    [BindProperty(Name = "_ajax")]
    public bool IsAjaxSubmit { get; set; }

    public List<CartItem> CartItems { get; set; } = new();
    public List<ChiNhanhShowroom> DanhSachChiNhanh { get; set; } = new();
    public Dictionary<int, List<TonKhoTheoChiNhanh>> TonKhoTheoPhienBan { get; set; } = new();
    public Dictionary<int, int> DaDatCocTheoPhienBanVaChiNhanh { get; set; } = new();
    public Dictionary<(int, string), int> TonKhoConLaiLut { get; set; } = new();
    public decimal TotalDeposit { get; set; }
    public string? ErrorMessage { get; set; }

    private Dictionary<(int, string), int> _reservedLut = new();
    private Dictionary<(int, string), int> _tonKhoLut = new();
    private Dictionary<string, string> _tenChiNhanhLut = new();

    public CheckoutModel(AppDbContext db, ICartService cart, IActivityLogService log, INotificationService notif)
    {
        _db = db;
        _cart = cart;
        _log = log;
        _notif = notif;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        CartItems = await _cart.GetCartAsync();
        if (CartItems.Count == 0)
            return RedirectToPage("/Orders/Cart/Index");

        if (!await _cart.HasMinimumQuantityAsync())
        {
            TempData["CartError"] = "Cần ít nhất 3 xe để đặt cọc theo giỏ hàng.";
            return RedirectToPage("/Orders/Cart/Index");
        }

        await LoadShowrooms();
        await LoadTonKhoAsync();
        await LoadReservedAsync();
        TotalDeposit = await _cart.GetTotalDepositAsync();
        QuantityInput = CartItems.ToDictionary(c => c.MaPhienBan, c => c.SoLuong);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CartItems = await _cart.GetCartAsync();
        if (CartItems.Count == 0)
            return RedirectToPage("/Orders/Cart/Index");

        if (!await _cart.HasMinimumQuantityAsync())
            return Fail("Cần ít nhất 3 xe để đặt cọc.");

        if (string.IsNullOrEmpty(HoTen) || string.IsNullOrEmpty(SoDienThoai) || string.IsNullOrEmpty(DiaChi))
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng điền đầy đủ thông tin.");
            ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
            await LoadShowrooms();
            return Page();
        }

        if (string.IsNullOrEmpty(MaChiNhanh))
        {
            if (IsAjaxSubmit) return JsonError("Vui lòng chọn showroom hẹn gặp / nhận xe.");
            ErrorMessage = "Vui lòng chọn showroom hẹn gặp / nhận xe.";
            await LoadShowrooms();
            await LoadTonKhoAsync();
            return Page();
        }

        var userName = User.GetJwtUserName();
        var groupCode = $"MLC{DateTime.Now:yyMMddHHmmss}";

        // Áp dụng số lượng người dùng đã chọn
        foreach (var item in CartItems)
        {
            if (QuantityInput.TryGetValue(item.MaPhienBan, out var qty) && qty > 0 && qty <= item.SoLuong)
            {
                item.SoLuong = qty;
            }
        }

        TotalDeposit = CartItems.Sum(c => c.SoTienCoc * c.SoLuong);

        var pbIds = CartItems.Select(c => c.MaPhienBan).Distinct().ToList();

        // ===== Transaction + khoá dòng để chống bán vượt tồn kho khi đặt cọc đồng thời =====
        await using var tx = await _db.Database.BeginTransactionAsync();

        // Khoá tất cả dòng tồn kho của các phiên bản trong đơn (UPDLOCK + HOLDLOCK giữ tới hết transaction)
        foreach (var pbId in pbIds)
        {
            await _db.Database.ExecuteSqlRawAsync(
                "SELECT MaTonKho FROM TonKhoTheoChiNhanh WITH (UPDLOCK, HOLDLOCK) WHERE MaPhienBan = {0}", pbId);
        }

        // Sau khi đã khoá, đọc lại tồn kho + số lượng đã đặt cọc
        await LoadTonKhoLutAsync(pbIds);
        await LoadReservedLutAsync(pbIds);
        await LoadTenChiNhanhLutAsync();

        // ===== Phân bổ showroom nguồn: ưu tiên showroom đã chọn / showroom hẹn gặp trước, sau đó mới sang showroom khác =====
        var allocated = new List<(CartItem item, List<(string cn, int qty)> parts)>();
        foreach (var item in CartItems)
        {
            var phienBan = await _db.PhienBanXe.FindAsync(item.MaPhienBan);
            if (phienBan == null)
            {
                ErrorMessage = $"Xe \"{item.TenPhienBan}\" không tồn tại.";
                await tx.RollbackAsync();
                await LoadShowrooms();
                return Page();
            }

            var userSource = SourceChiNhanh.TryGetValue(item.MaPhienBan, out var sc) && !string.IsNullOrEmpty(sc)
                ? sc
                : null;

            var parts = AllocateSource(phienBan, item.SoLuong, userSource, MaChiNhanh);
            if (parts.Count == 0)
            {
                var available = phienBan.SoLuongTrongKho <= 0
                    ? 0
                    : _tonKhoLut.Where(k => k.Key.Item1 == item.MaPhienBan).Sum(k => k.Value)
                        - _reservedLut.Where(k => k.Key.Item1 == item.MaPhienBan).Sum(k => k.Value);
                ErrorMessage = $"Không đủ xe \"{item.TenPhienBan}\". Hiện chỉ còn {Math.Max(0, available)} xe trên toàn hệ thống.";
                await tx.RollbackAsync();
                await LoadShowrooms();
                await LoadTonKhoAsync();
                return Page();
            }

            allocated.Add((item, parts));
        }

        var totalXe = CartItems.Sum(c => c.SoLuong);
        var hasPreOrder = CartItems.Any(c => c.SoLuongTrongKho <= 0);

        // ===== Tạo MỘT đơn cọc duy nhất =====
        // Xe hết hàng (đặt trước) -> Chờ xử lý; xe còn hàng -> Chờ xác nhận
        var deposit = new DonDatCoc
        {
            MaKhachHang = userName ?? "",
            MaChiNhanh = MaChiNhanh,
            SoTienCoc = TotalDeposit,
            PhuongThucThanhToan = PhuongThucThanhToan,
            TrangThaiThanhToan = "Chưa thanh toán",
            TrangThaiDonHang = hasPreOrder ? "Chờ xử lý" : "Chờ xác nhận",
            NgayTaoDon = DateTime.Now,
            HoTen = HoTen,
            SoDienThoai = SoDienThoai,
            DiaChi = DiaChi,
            GhiChu = GhiChu,
            MaGiaoDich = groupCode
        };

        // Thêm chi tiết từng xe (một xe có thể chia nhiều showroom nguồn) vào chung một đơn
foreach (var (item, parts) in allocated)
        {
            foreach (var (cn, qty) in parts)
            {
                var isPreOrder = item.SoLuongTrongKho <= 0;
                _tonKhoLut.TryGetValue((item.MaPhienBan, cn), out var tonKhoCn);
                _reservedLut.TryGetValue((item.MaPhienBan, cn), out var reservedCn);
                var soLuongThieu = isPreOrder ? qty : Math.Max(0, qty - Math.Max(0, tonKhoCn - reservedCn));

                deposit.ChiTiets.Add(new DonDatCocChiTiet
                {
                    MaPhienBan = item.MaPhienBan,
                    MaChiNhanh = cn,
                    SoLuong = qty,
                    SoTienCocPhanBo = DepositCalculator.Compute(item.GiaNiemYet, isPreOrder) * qty,
                    SoLuongThieu = soLuongThieu
                });
            }
        }

        _db.DonDatCoc.Add(deposit);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        // Tạo lịch hẹn nhận xe tại showroom hẹn gặp nếu có chọn
        if (!string.IsNullOrEmpty(LichHenNgay) && !string.IsNullOrEmpty(LichHenGio))
        {
            var lichHen = new LichHenLaiThu
            {
                MaKhachHang = userName ?? "",
                MaDong = CartItems.FirstOrDefault()?.MaPhienBan ?? 0,
                MaChiNhanh = MaChiNhanh,
                HoTenNguoiLai = HoTen,
                SoDienThoai = SoDienThoai,
                SoBangLaiXe = "",
                NgayHen = DateTime.Parse(LichHenNgay),
                GioHen = LichHenGio,
                TrangThai = "Chờ xác nhận",
                YKienKhachHang = GhiChu ?? ""
            };
            _db.LichHenLaiThu.Add(lichHen);
            await _db.SaveChangesAsync();
        }

        await _log.LogAsync("Đặt cọc giỏ hàng",
            $"{HoTen} - {SoDienThoai} - {totalXe} xe - Tổng cọc: {TotalDeposit:N0}VNĐ - Nhận tại: {MaChiNhanh}");

        // Giữ nguyên giỏ hàng; chỉ xoá khi thanh toán thành công hoặc người dùng tự xoá
        if (IsAjaxSubmit)
        {
            var sepay = HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<CarProject.Services.SepaySettings>>();
            var s = sepay.Value;
            var bin = "970422";
            var qrUrl = CarProject.Services.VietQr.BuildDataUri(bin, s.BankNumber, 10000, groupCode);
            var tongTienGoc = CartItems.Sum(c => (long)c.GiaNiemYet * c.SoLuong);
            var rateList = CartItems.Select(c => c.SoLuongTrongKho <= 0 ? "15%" : "20%").Distinct().ToList();
            var ajaxRateText = rateList.Count == 1 ? rateList[0] : "15%/20%";
            return new JsonResult(new
            {
                success = true,
                maDonCoc = deposit.MaDonCoc,
                maGiaoDich = groupCode,
                soTienCoc = TotalDeposit,
                tongTienGoc,
                cocRateText = ajaxRateText,
                bankName = $"{s.BankAccount} ({s.BankName})",
                bankNumber = s.BankNumber,
                accountName = s.AccountName,
                qrUrl
            });
        }

        return RedirectToPage("/Orders/DepositResult", new { maDonCoc = deposit.MaDonCoc });
    }

    // ===== Phân bổ nguồn: ưu tiên showroom đã chọn -> showroom hẹn gặp -> các showroom còn lại =====
    private List<(string cn, int qty)> AllocateSource(PhienBanXe pb, int qty, string? userSource, string receiveCn)
    {
        // Xe hết hàng: cho phép đặt trước, chọn bất kỳ showroom để chuẩn bị xe
        if (pb.SoLuongTrongKho <= 0)
        {
            var src = !string.IsNullOrEmpty(userSource) ? userSource : receiveCn;
            return new List<(string, int)> { (src, qty) };
        }

        var candidates = new List<string>();
        if (!string.IsNullOrEmpty(userSource)) candidates.Add(userSource);
        if (!string.IsNullOrEmpty(receiveCn) && !candidates.Contains(receiveCn)) candidates.Add(receiveCn);

        var otherCn = _tonKhoLut
            .Where(k => k.Key.Item1 == pb.MaPhienBan && k.Value > 0 && !candidates.Contains(k.Key.Item2))
            .OrderByDescending(k => k.Value)
            .Select(k => k.Key.Item2);
        candidates.AddRange(otherCn);

        var result = new List<(string, int)>();
        var remaining = qty;
        foreach (var cn in candidates)
        {
            _tonKhoLut.TryGetValue((pb.MaPhienBan, cn), out var tonKho);
            _reservedLut.TryGetValue((pb.MaPhienBan, cn), out var reserved);
            var available = tonKho - reserved;
            if (available <= 0) continue;
            var take = Math.Min(remaining, available);
            result.Add((cn, take));
            remaining -= take;
            if (remaining <= 0) break;
        }

        return remaining > 0 ? new List<(string, int)>() : result;
    }

    private async Task LoadShowrooms()
    {
        DanhSachChiNhanh = await _db.ChiNhanhShowroom
            .Where(c => c.TrangThai == "Active" || c.TrangThai == "Hoạt động")
            .ToListAsync();
        foreach (var cn in DanhSachChiNhanh)
            _tenChiNhanhLut[cn.MaChiNhanh] = cn.TenChiNhanh;
    }

    private async Task LoadTonKhoAsync()
    {
        var maPhienBans = CartItems.Select(c => c.MaPhienBan).ToList();
        var tonKhoList = await _db.TonKhoTheoChiNhanh
            .Include(t => t.ChiNhanh)
            .Where(t => maPhienBans.Contains(t.MaPhienBan))
            .ToListAsync();
        TonKhoTheoPhienBan = tonKhoList.GroupBy(t => t.MaPhienBan)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task LoadReservedAsync()
    {
        var maPhienBans = CartItems.Select(c => c.MaPhienBan).ToList();
        await LoadReservedLutAsync(maPhienBans);
        foreach (var kv in _reservedLut)
        {
            if (!DaDatCocTheoPhienBanVaChiNhanh.ContainsKey(kv.Key.Item1))
                DaDatCocTheoPhienBanVaChiNhanh[kv.Key.Item1] = 0;
            DaDatCocTheoPhienBanVaChiNhanh[kv.Key.Item1] += kv.Value;
            TonKhoConLaiLut[kv.Key] = (_tonKhoLut.TryGetValue(kv.Key, out var tk) ? tk : 0) - kv.Value;
        }
        foreach (var tk in _tonKhoLut)
        {
            if (!_reservedLut.ContainsKey(tk.Key))
                TonKhoConLaiLut[tk.Key] = tk.Value;
        }
    }

    private async Task LoadReservedLutAsync(List<int> pbIds)
    {
        // Chỉ đếm 'đã đặt cọc' (giữ xe) cho những đơn ĐÃ THANH TOÁN.
        var rows = await _db.DonDatCocChiTiet
            .Where(c => pbIds.Contains(c.MaPhienBan)
                && c.DonDatCoc != null
                && c.DonDatCoc!.TrangThaiThanhToan == "Đã thanh toán")
            .Select(c => new { c.MaPhienBan, c.MaChiNhanh, c.SoLuong })
            .ToListAsync();
        _reservedLut = new Dictionary<(int, string), int>();
        foreach (var r in rows)
        {
            var key = (r.MaPhienBan, r.MaChiNhanh ?? "");
            _reservedLut.TryGetValue(key, out var cur);
            _reservedLut[key] = cur + r.SoLuong;
        }
    }

    private async Task LoadTonKhoLutAsync(List<int> pbIds)
    {
        var rows = await _db.TonKhoTheoChiNhanh
            .Where(t => pbIds.Contains(t.MaPhienBan))
            .ToListAsync();
        _tonKhoLut = new Dictionary<(int, string), int>();
        foreach (var r in rows)
        {
            _tonKhoLut[(r.MaPhienBan, r.MaChiNhanh)] = r.SoLuong;
        }
    }

    private async Task LoadTenChiNhanhLutAsync()
    {
        var rows = await _db.ChiNhanhShowroom.ToListAsync();
        _tenChiNhanhLut = rows.ToDictionary(c => c.MaChiNhanh, c => c.TenChiNhanh);
    }

    private string TenChiNhanh(string? maCn)
        => _tenChiNhanhLut.TryGetValue(maCn ?? "", out var t) ? t : (maCn ?? "");

    private IActionResult JsonError(string message)
        => new JsonResult(new { success = false, error = message });

    private IActionResult Fail(string message)
    {
        if (IsAjaxSubmit) return JsonError(message);
        ErrorMessage = message;
        return Page();
    }
}
