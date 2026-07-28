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

    public List<CartItem> CartItems { get; set; } = new();
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
            return Page();
        }

        var userName = User.GetJwtUserName();
        var groupCode = $"DH{DateTime.Now:yyMMddHHmmss}";

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
            $"{HoTen} đặt cọc {totalXe} xe - {TotalDeposit:N0} VNĐ",
            $"/Admin/DonCoc/Index");

        var showroom = await _db.ChiNhanhShowroom
            .Where(c => c.MaQuanLy != null && c.TrangThai == "Active")
            .FirstOrDefaultAsync();

        if (showroom != null)
        {
            await _notif.SendAsync(showroom.MaQuanLy, "Đơn cọc giỏ hàng mới",
                $"{HoTen} đặt cọc {totalXe} xe - {TotalDeposit:N0} VNĐ",
                $"/QuanLy/Dashboard");
        }

        await _log.LogAsync("Đặt cọc giỏ hàng",
            $"{HoTen} - {SoDienThoai} - {totalXe} xe - Tổng cọc: {TotalDeposit:N0}VNĐ");

        await _cart.ClearCartAsync();

        TempData["CartCheckoutResult"] = JsonSerializer.Serialize(new
        {
            hoTen = HoTen,
            soDienThoai = SoDienThoai,
            soLuongXe = totalXe,
            totalDeposit = TotalDeposit,
            maDonCocs = createdDeposits,
            phuongThucThanhToan = PhuongThucThanhToan
        });

        return RedirectToPage("/Orders/Cart/CheckoutResult");
    }
}
