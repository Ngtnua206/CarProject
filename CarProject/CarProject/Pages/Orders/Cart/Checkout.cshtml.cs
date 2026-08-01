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

        TotalDeposit = CartItems.Sum(c => c.SoTienCoc * c.SoLuong);

        // Kiểm tra tồn kho theo showroom nguồn trước khi đặt cọc
        var reservedStatuses = new List<string> { "Chờ xử lý", "Chờ xác nhận", "Đã xác nhận", "Đã thanh toán", "Hoàn tất" };
        foreach (var item in CartItems)
        {
            var phienBan = await _db.PhienBanXe.FindAsync(item.MaPhienBan);
            if (phienBan == null)
            {
                ErrorMessage = $"Xe \"{item.TenPhienBan}\" không tồn tại.";
                await LoadShowrooms();
                return Page();
            }

            var sourceCn = SourceChiNhanh.TryGetValue(item.MaPhienBan, out var sc) && !string.IsNullOrEmpty(sc)
                ? sc
                : MaChiNhanh;

            // Xe hết hàng được phép đặt trước (chọn showroom bất kỳ để chuẩn bị xe)
            if (phienBan.SoLuongTrongKho <= 0)
                continue;

            // Tính số xe đã đặt cọc (mọi trạng thái trừ Đã từ chối)
            var reservedQty = await _db.DonDatCocChiTiet
                .CountAsync(ct => ct.MaPhienBan == item.MaPhienBan
                    && ct.MaChiNhanh == sourceCn
                    && reservedStatuses.Contains(ct.DonDatCoc!.TrangThaiDonHang ?? ""));

            var tonKho = await _db.TonKhoTheoChiNhanh
                .FirstOrDefaultAsync(t => t.MaPhienBan == item.MaPhienBan && t.MaChiNhanh == sourceCn);
            var available = (tonKho?.SoLuong ?? 0) - reservedQty;

            if (item.SoLuong > available)
            {
                ErrorMessage = $"Showroom \"{sourceCn}\" không đủ xe \"{item.TenPhienBan}\". Hiện chỉ còn {available} xe.";
                await LoadShowrooms();
                return Page();
            }
        }

        var totalXe = CartItems.Sum(c => c.SoLuong);

        // Tạo MỘT đơn cọc duy nhất kèm thông tin người đặt
        var deposit = new DonDatCoc
        {
            MaKhachHang = userName ?? "",
            MaChiNhanh = MaChiNhanh,
            SoTienCoc = TotalDeposit,
            PhuongThucThanhToan = PhuongThucThanhToan,
            TrangThaiThanhToan = "Chưa thanh toán",
            TrangThaiDonHang = "Chờ xử lý",
            NgayTaoDon = DateTime.Now,
            HoTen = HoTen,
            SoDienThoai = SoDienThoai,
            DiaChi = DiaChi,
            GhiChu = GhiChu,
            MaGiaoDich = groupCode
        };

        // Thêm chi tiết từng xe vào chung một đơn, mỗi xe có showroom nguồn riêng
        foreach (var item in CartItems)
        {
            var sourceCn = SourceChiNhanh.TryGetValue(item.MaPhienBan, out var sc) && !string.IsNullOrEmpty(sc)
                ? sc
                : MaChiNhanh;
            deposit.ChiTiets.Add(new DonDatCocChiTiet
            {
                MaPhienBan = item.MaPhienBan,
                MaChiNhanh = sourceCn,
                SoLuong = item.SoLuong
            });
        }

        _db.DonDatCoc.Add(deposit);
        await _db.SaveChangesAsync();

        // Tạo lịch hẹn nhận xe tại showroom nhận nếu có chọn
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

        var tenXeList = CartItems.Select(c => c.TenPhienBan).Distinct().ToList();
        var tenXeStr = string.Join(", ", tenXeList);

        await _log.LogAsync("Đặt cọc giỏ hàng",
            $"{HoTen} - {SoDienThoai} - {totalXe} xe - Tổng cọc: {TotalDeposit:N0}VNĐ - Nhận tại: {MaChiNhanh}");

        // Gửi thông báo chung cho Admin
        await _notif.SendToRoleAsync("Admin", "Đơn đặt cọc mới",
            $"Đơn cọc #{deposit.MaDonCoc} - {HoTen} - {totalXe} xe - Tổng cọc: {TotalDeposit:N0}đ - Nhận tại {MaChiNhanh}",
            $"/Admin/DonCoc/Edit?maDonCoc={deposit.MaDonCoc}");

        // Gửi thông báo tới từng showroom có xe trong đơn
        foreach (var group in deposit.ChiTiets.GroupBy(c => c.MaChiNhanh))
        {
            var cn = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaChiNhanh == group.Key);
            if (cn?.MaQuanLy == null) continue;

            var soLuongCn = group.Sum(c => c.SoLuong);
            string noiDung;
            if (group.Key == MaChiNhanh)
            {
                noiDung = $"Khách {HoTen} muốn mua {soLuongCn} xe tại showroom {cn.TenChiNhanh} của bạn. Vui lòng xác nhận tiếp nhận.";
            }
            else
            {
                noiDung = $"Khách {HoTen} muốn mua {soLuongCn} xe tại showroom {cn.TenChiNhanh} của bạn và nhận xe tại {MaChiNhanh}. Bạn có tiếp nhận vận chuyển xe tới showroom nhận không?";
            }
            await _notif.SendAsync(cn.MaQuanLy, "Đơn đặt cọc mới - cần xác nhận",
                $"{noiDung} (Đơn cọc #{deposit.MaDonCoc})",
                $"/QuanLy/DonCoc?highlight={deposit.MaDonCoc}");
        }

        await _cart.ClearCartAsync();

        if (IsAjaxSubmit)
        {
            var sepay = HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<CarProject.Services.SepaySettings>>();
            var s = sepay.Value;
            var bin = "970436";
            var qrUrl = $"https://img.vietqr.io/image/{bin}-{s.BankNumber}-compact2.jpg?amount=10000&addInfo={Uri.EscapeDataString(groupCode)}&accountName={Uri.EscapeDataString(s.AccountName)}";
            return new JsonResult(new
            {
                success = true,
                maDonCoc = deposit.MaDonCoc,
                maGiaoDich = groupCode,
                soTienCoc = TotalDeposit,
                bankName = $"{s.BankAccount} ({s.BankName})",
                bankNumber = s.BankNumber,
                accountName = s.AccountName,
                qrUrl
            });
        }

        return RedirectToPage("/Orders/DepositResult", new { maDonCoc = deposit.MaDonCoc });
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
            .Where(t => maPhienBans.Contains(t.MaPhienBan) && t.SoLuong > 0)
            .ToListAsync();
        TonKhoTheoPhienBan = tonKhoList.GroupBy(t => t.MaPhienBan)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}