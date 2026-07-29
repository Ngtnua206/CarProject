using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarProject.Pages.Orders.Cart;

public class CheckoutResultModel : PageModel
{
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

    public CheckoutResultModel(IOptions<SepaySettings> sepay)
    {
        _sepay = sepay.Value;
    }

    public IActionResult OnGet()
    {
        var raw = TempData["CartCheckoutResult"] as string;
        if (string.IsNullOrEmpty(raw))
            return RedirectToPage("/Orders/Cart/Index");

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

        TotalDepositStr = TotalDeposit >= 1_000_000_000
            ? $"{TotalDeposit / 1_000_000_000:N2} tỷ VNĐ"
            : $"{TotalDeposit / 1_000_000:N0} triệu VNĐ";

        BankName = $"{_sepay.BankAccount} ({_sepay.BankName})";
        BankNumber = _sepay.BankNumber;
        AccountName = _sepay.AccountName;

        return Page();
    }
}
