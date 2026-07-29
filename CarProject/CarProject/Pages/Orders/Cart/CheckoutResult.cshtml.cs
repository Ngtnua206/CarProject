using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarProject.Pages.Orders.Cart;

public class CheckoutResultModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly SepaySettings _sepay;

    public string HoTen { get; set; } = "";
    public string SoDienThoai { get; set; } = "";
    public int SoLuongXe { get; set; }
    public decimal TotalDeposit { get; set; }
    public string TotalDepositStr { get; set; } = "";
    public string PhuongThucThanhToan { get; set; } = "";
    public List<int> MaDonCocs { get; set; } = new();
    public string MaGiaoDich { get; set; } = "";
    public string BankName { get; set; } = "";
    public string BankNumber { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string QrImageUrl { get; set; } = "";
    public bool ShowQr { get; set; }

    public CheckoutResultModel(AppDbContext db, IOptions<SepaySettings> sepay)
    {
        _db = db;
        _sepay = sepay.Value;
    }

    public async Task<IActionResult> OnGet(int? maDonCoc)
    {
        var raw = TempData["CartCheckoutResult"] as string;
        if (!string.IsNullOrEmpty(raw))
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            HoTen = root.GetProperty("hoTen").GetString() ?? "";
            SoDienThoai = root.GetProperty("soDienThoai").GetString() ?? "";
            SoLuongXe = root.GetProperty("soLuongXe").GetInt32();
            TotalDeposit = root.GetProperty("totalDeposit").GetDecimal();
            PhuongThucThanhToan = root.GetProperty("phuongThucThanhToan").GetString() ?? "";
            MaGiaoDich = root.TryGetProperty("maGiaoDich", out var mgd) ? mgd.GetString() ?? "" : "";

            var ids = root.GetProperty("maDonCocs");
            foreach (var id in ids.EnumerateArray())
                MaDonCocs.Add(id.GetInt32());
        }
        else if (maDonCoc.HasValue)
        {
            var don = await _db.DonDatCoc
                .Include(d => d.PhienBan).ThenInclude(p => p.DongXe)
                .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc);
            if (don == null) return RedirectToPage("/Index");

            HoTen = don.HoTen ?? "";
            SoDienThoai = don.SoDienThoai ?? "";
            SoLuongXe = 1;
            TotalDeposit = don.SoTienCoc;
            PhuongThucThanhToan = don.PhuongThucThanhToan ?? "Chuyển khoản";
            MaGiaoDich = don.MaGiaoDich ?? "";
            MaDonCocs = new List<int> { don.MaDonCoc };
        }
        else
        {
            return RedirectToPage("/Orders/Cart/Index");
        }

        TotalDepositStr = TotalDeposit >= 1_000_000_000
            ? $"{TotalDeposit / 1_000_000_000:N2} tỷ VNĐ"
            : $"{TotalDeposit / 1_000_000:N0} triệu VNĐ";

        BankName = $"{_sepay.BankAccount} ({_sepay.BankName})";
        BankNumber = _sepay.BankNumber;
        AccountName = _sepay.AccountName;

        if (PhuongThucThanhToan == "Chuyển khoản" && !string.IsNullOrEmpty(MaGiaoDich))
        {
            var bin = "970436";
            var encodedInfo = Uri.EscapeDataString(MaGiaoDich);
            var encodedName = Uri.EscapeDataString(_sepay.AccountName);
            QrImageUrl = $"https://img.vietqr.io/image/{bin}-{_sepay.BankNumber}-compact2.jpg?amount=10000&addInfo={encodedInfo}&accountName={encodedName}";
            ShowQr = true;
        }

        return Page();
    }
}
