using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CarProject.Pages.Orders;

public class DepositResultModel : PageModel
{
    public int MaDonCoc { get; set; }
    public string MaGiaoDich { get; set; } = "";
    public decimal SoTienCoc { get; set; }
    public string SoTienCocStr { get; set; } = "";
    public string BankName { get; set; } = "";
    public string BankNumber { get; set; } = "";
    public string AccountName { get; set; } = "";
    public string TransferContent { get; set; } = "";
    public string QrImageUrl { get; set; } = "";
    public string TenPhienBan { get; set; } = "";
    public string HoTen { get; set; } = "";

    public IActionResult OnGet()
    {
        var raw = TempData["DepositResult"] as string;
        if (string.IsNullOrEmpty(raw))
            return RedirectToPage("/Index");

        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;

        MaDonCoc = root.GetProperty("maDonCoc").GetInt32();
        MaGiaoDich = root.GetProperty("maGiaoDich").GetString() ?? "";
        SoTienCoc = root.GetProperty("soTienCoc").GetDecimal();
        BankName = root.GetProperty("bankName").GetString() ?? "";
        BankNumber = root.GetProperty("bankNumber").GetString() ?? "";
        AccountName = root.GetProperty("accountName").GetString() ?? "";
        TransferContent = root.GetProperty("transferContent").GetString() ?? "";
        QrImageUrl = root.GetProperty("qrImageUrl").GetString() ?? "";
        TenPhienBan = root.GetProperty("tenPhienBan").GetString() ?? "";
        HoTen = root.GetProperty("hoTen").GetString() ?? "";

        SoTienCocStr = SoTienCoc >= 1_000_000_000
            ? $"{SoTienCoc / 1_000_000_000:N1} tỷ VNĐ"
            : $"{SoTienCoc / 1_000_000:N0} triệu VNĐ";

        return Page();
    }
}
