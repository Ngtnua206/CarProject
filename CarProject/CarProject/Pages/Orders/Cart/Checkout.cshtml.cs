using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;
using System.Text.Json;

namespace CarProject.Pages.Orders.Cart;

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

        // Kiểm tra tồn kho trước khi đặt cọc
        foreach (var item in CartItems)
        {
            var phienBan = await _db.PhienBanXe.FindAsync(item.MaPhienBan);
            if (phienBan == null)
            {
                ErrorMessage = $"Xe \"{item.TenPhienBan}\" không tồn tại.";
                await LoadShowrooms();
                return Page();
            }
            // Chỉ tính xe đã đặt cọc thành công (đã thanh toán) là đã giữ
            var reservedQty = await _db.DonDatCoc
                .CountAsync(d => d.MaPhienBan == item.MaPhienBan
                    && (d.TrangThaiDonHang == "Chờ xác nhận" || d.TrangThaiDonHang == "Đã xác nhận"));
            var available = phienBan.SoLuongTrongKho - reservedQty;
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

        if (userName != null)
        {
            await _notif.SendAsync(userName, "Đặt cọc giỏ hàng thành công",
                $"{totalXe} xe - Tổng cọc: {TotalDeposit:N0} VNĐ. Mã đơn: #{string.Join(", #", createdDeposits)}",
                $"/Orders/Cart/CheckoutResult");
        }

        await _notif.SendToRoleAsync("Admin", "Đơn cọc giỏ hàng mới",
            $"{HoTen} đặt cọc {totalXe} xe - {TotalDeposit:N0} VNĐ tại showroom",
            $"/Admin/DonCoc/Index");

        var showroomManager = await _db.ChiNhanhShowroom
            .Where(c => c.MaChiNhanh == MaChiNhanh && c.MaQuanLy != null)
            .Select(c => c.MaQuanLy)
            .FirstOrDefaultAsync();

        if (showroomManager != null)
        {
            await _notif.SendAsync(showroomManager, "Đơn cọc giỏ hàng mới",
                $"{HoTen} đặt cọc {totalXe} xe - {TotalDeposit:N0} VNĐ tại chi nhánh của bạn",
                $"/QuanLy/DonCoc");
        }

        await _log.LogAsync("Đặt cọc giỏ hàng",
            $"{HoTen} - {SoDienThoai} - {totalXe} xe - Tổng cọc: {TotalDeposit:N0}VNĐ - Showroom: {MaChiNhanh}");

        await _cart.ClearCartAsync();

        TempData["CartCheckoutResult"] = JsonSerializer.Serialize(new
        {
            hoTen = HoTen,
            soDienThoai = SoDienThoai,
            soLuongXe = totalXe,
            totalDeposit = TotalDeposit,
            maDonCocs = createdDeposits,
            phuongThucThanhToan = PhuongThucThanhToan,
            maChiNhanh = MaChiNhanh,
            maGiaoDich = groupCode
        });

        return RedirectToPage("/Orders/Cart/CheckoutResult");
    }

    private async Task LoadShowrooms()
    {
        DanhSachChiNhanh = await _db.ChiNhanhShowroom
            .Where(c => c.TrangThai == "Active" || c.TrangThai == "Hoạt động")
            .ToListAsync();
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
}