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

    [BindProperty(Name = "_ajax")]
    public bool IsAjaxSubmit { get; set; }

    public List<CartItem> CartItems { get; set; } = new();
    public List<ChiNhanhShowroom> DanhSachChiNhanh { get; set; } = new();
    public Dictionary<int, List<TonKhoTheoChiNhanh>> TonKhoTheoPhienBan { get; set; } = new();
    public decimal TotalDeposit { get; set; }
    public string? ErrorMessage { get; set; }

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
        {
            ErrorMessage = "Cần ít nhất 3 xe để đặt cọc.";
            return Page();
        }

        if (string.IsNullOrEmpty(HoTen) || string.IsNullOrEmpty(SoDienThoai) || string.IsNullOrEmpty(DiaChi))
        {
            ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
            await LoadShowrooms();
            return Page();
        }

        if (string.IsNullOrEmpty(MaChiNhanh))
        {
            ErrorMessage = "Vui lòng chọn showroom nhận xe.";
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

        // Kiểm tra tồn kho trước khi đặt cọc
        var reservedStatuses = new[] { "Chờ xử lý", "Chờ xác nhận", "Đã xác nhận", "Đã thanh toán", "Hoàn tất" };
        foreach (var item in CartItems)
        {
            var phienBan = await _db.PhienBanXe.FindAsync(item.MaPhienBan);
            if (phienBan == null)
            {
                ErrorMessage = $"Xe \"{item.TenPhienBan}\" không tồn tại.";
                await LoadShowrooms();
                return Page();
            }

            // Tính số xe đã đặt cọc (mọi trạng thái trừ Đã từ chối)
            var reservedQty = await _db.DonDatCoc
                .CountAsync(d => d.MaPhienBan == item.MaPhienBan
                    && reservedStatuses.Contains(d.TrangThaiDonHang ?? ""));

            // Kiểm tra theo showroom nếu có chọn
            int available;
            if (!string.IsNullOrEmpty(MaChiNhanh))
            {
                var tonKho = await _db.TonKhoTheoChiNhanh
                    .FirstOrDefaultAsync(t => t.MaPhienBan == item.MaPhienBan && t.MaChiNhanh == MaChiNhanh);
                available = (tonKho?.SoLuong ?? 0) - reservedQty;
            }
            else
            {
                available = phienBan.SoLuongTrongKho - reservedQty;
            }

            if (item.SoLuong > available)
            {
                ErrorMessage = $"Số lượng xe \"{item.TenPhienBan}\" trong kho không đủ. Hiện chỉ còn {available} xe.";
                await LoadShowrooms();
                return Page();
            }
        }

        var createdDeposits = new List<int>();
        var totalXe = CartItems.Sum(c => c.SoLuong);
        var tenXeList = new List<string>();

        foreach (var item in CartItems)
        {
            for (int i = 0; i < item.SoLuong; i++)
            {
                var deposit = new DonDatCoc
                {
                    MaKhachHang = userName ?? "",
                    MaPhienBan = item.MaPhienBan,
                    MaChiNhanh = MaChiNhanh,
                    SoTienCoc = item.SoTienCoc,
                    PhuongThucThanhToan = PhuongThucThanhToan,
                    TrangThaiThanhToan = "Chưa thanh toán",
                    TrangThaiDonHang = "Chờ xử lý",
                    NgayTaoDon = DateTime.Now,
                    HoTen = HoTen,
                    SoDienThoai = SoDienThoai,
                    DiaChi = DiaChi,
                    GhiChu = GhiChu,
                    MaGiaoDich = $"{groupCode}-{item.MaPhienBan}-{i + 1}"
                };

                _db.DonDatCoc.Add(deposit);
                await _db.SaveChangesAsync();
                createdDeposits.Add(deposit.MaDonCoc);
                tenXeList.Add(item.TenPhienBan);
            }
        }

        var tenXeStr = string.Join(", ", tenXeList.Distinct());

        await _log.LogAsync("Đặt cọc giỏ hàng",
            $"{HoTen} - {SoDienThoai} - {totalXe} xe - Tổng cọc: {TotalDeposit:N0}VNĐ - Showroom: {MaChiNhanh}");

        await _cart.ClearCartAsync();

        if (IsAjaxSubmit)
        {
            var first = createdDeposits.FirstOrDefault();
            var sepay = HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<CarProject.Services.SepaySettings>>();
            var s = sepay.Value;
            var bin = "970436";
            var qrUrl = $"https://img.vietqr.io/image/{bin}-{s.BankNumber}-compact2.jpg?amount=10000&addInfo={Uri.EscapeDataString(groupCode)}&accountName={Uri.EscapeDataString(s.AccountName)}";
            return new JsonResult(new
            {
                success = true,
                maDonCoc = first,
                maGiaoDich = groupCode,
                soTienCoc = TotalDeposit,
                bankName = $"{s.BankAccount} ({s.BankName})",
                bankNumber = s.BankNumber,
                accountName = s.AccountName,
                qrUrl
            });
        }

        return RedirectToPage("/Orders/Payment", new { maDonCoc = createdDeposits.FirstOrDefault() });
    }

    private async Task LoadShowrooms()
    {
        var maPhienBans = CartItems.Select(c => c.MaPhienBan).ToList();

        var showroomCoTonKho = await _db.TonKhoTheoChiNhanh
            .Where(t => maPhienBans.Contains(t.MaPhienBan) && t.SoLuong > 0)
            .Select(t => t.MaChiNhanh)
            .Distinct()
            .ToListAsync();

        DanhSachChiNhanh = await _db.ChiNhanhShowroom
            .Where(c => (c.TrangThai == "Active" || c.TrangThai == "Hoạt động")
                && showroomCoTonKho.Contains(c.MaChiNhanh))
            .ToListAsync();
    }

    private async Task LoadTonKhoAsync()
    {
        var maPhienBans = CartItems.Select(c => c.MaPhienBan).ToList();
        var tonKhoList = await _db.TonKhoTheoChiNhanh
            .Include(t => t.ChiNhanh)
            .Where(t => maPhienBans.Contains(t.MaPhienBan) && t.SoLuong > 0)
            .ToListAsync();
        TonKhoTheoPhienBan = tonKhoList.GroupBy(t => t.MaPhienBan)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}