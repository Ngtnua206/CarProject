using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarProject.Pages.Orders;

public class DepositResultModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly SepaySettings _sepay;

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
    public bool ShowQr { get; set; }
    public string DongCo { get; set; } = "";
    public string HopSo { get; set; } = "";
    public string MauSac { get; set; } = "";
    public string LoaiNhietLieu { get; set; } = "";
    public long GiaNiemYet { get; set; }
    public string GiaNiemYetStr { get; set; } = "";
    public string TrangThaiDonHang { get; set; } = "";
    public string SoDienThoai { get; set; } = "";

    public DepositResultModel(AppDbContext db, IOptions<SepaySettings> sepay)
    {
        _db = db;
        _sepay = sepay.Value;
    }

    public async Task<IActionResult> OnGetAsync(int? maDonCoc)
    {
        var raw = TempData["DepositResult"] as string;
        if (!string.IsNullOrEmpty(raw))
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            MaDonCoc = root.GetProperty("maDonCoc").GetInt32();
            MaGiaoDich = root.GetProperty("maGiaoDich").GetString() ?? "";
            SoTienCoc = root.GetProperty("soTienCoc").GetDecimal();
            BankName = root.GetProperty("bankName").GetString() ?? "";
            BankNumber = root.GetProperty("bankNumber").GetString() ?? "";
            AccountName = root.GetProperty("accountName").GetString() ?? "";
            TransferContent = root.GetProperty("transferContent").GetString() ?? "";
            TenPhienBan = root.GetProperty("tenPhienBan").GetString() ?? "";
            HoTen = root.GetProperty("hoTen").GetString() ?? "";
            SoDienThoai = root.TryGetProperty("soDienThoai", out var sdt) ? sdt.GetString() ?? "" : "";
            DongCo = root.TryGetProperty("dongCo", out var dc) ? dc.GetString() ?? "" : "";
            HopSo = root.TryGetProperty("hopSo", out var hs) ? hs.GetString() ?? "" : "";
            MauSac = root.TryGetProperty("mauSac", out var ms) ? ms.GetString() ?? "" : "";
            LoaiNhietLieu = root.TryGetProperty("loaiNhietLieu", out var lnl) ? lnl.GetString() ?? "" : "";
            GiaNiemYet = root.TryGetProperty("giaNiemYet", out var gny) ? gny.GetInt64() : 0;
        }
        else if (maDonCoc.HasValue)
        {
            // Load từ DB nếu vào từ notification link
            var don = await _db.DonDatCoc
                .Include(d => d.PhienBan).ThenInclude(p => p.DongXe)
                .FirstOrDefaultAsync(d => d.MaDonCoc == maDonCoc.Value);
            if (don == null) return RedirectToPage("/Index");

            MaDonCoc = don.MaDonCoc;
            MaGiaoDich = don.MaGiaoDich ?? "";
            SoTienCoc = don.SoTienCoc;
            BankName = $"{_sepay.BankAccount} ({_sepay.BankName})";
            BankNumber = _sepay.BankNumber;
            AccountName = _sepay.AccountName;
            TransferContent = don.MaGiaoDich ?? "";
            TenPhienBan = $"{don.PhienBan?.DongXe?.TenDong ?? ""} {don.PhienBan?.TenPhienBan ?? ""}".Trim();
            HoTen = don.HoTen ?? "";
            SoDienThoai = don.SoDienThoai ?? "";
            TrangThaiDonHang = don.TrangThaiDonHang ?? "";
            if (don.PhienBan != null)
            {
                DongCo = don.PhienBan.DongCo ?? "";
                HopSo = don.PhienBan.HopSo ?? "";
                MauSac = don.PhienBan.MauSac ?? "";
                LoaiNhietLieu = don.PhienBan.LoaiNhietLieu ?? "";
                GiaNiemYet = don.PhienBan.GiaNiemYet;
            }
        }
        else
        {
            return RedirectToPage("/Index");
        }

        SoTienCocStr = SoTienCoc >= 1_000_000_000
            ? $"{SoTienCoc / 1_000_000_000:N1} tỷ VNĐ"
            : $"{SoTienCoc / 1_000_000:N0} triệu VNĐ";
        GiaNiemYetStr = GiaNiemYet >= 1_000_000_000
            ? $"{GiaNiemYet / 1_000_000_000:N1} tỷ VNĐ"
            : $"{GiaNiemYet / 1_000_000:N0} triệu VNĐ";

        if (!string.IsNullOrEmpty(MaGiaoDich))
        {
            var bin = "970436"; // VQR bin
            var encodedInfo = Uri.EscapeDataString(MaGiaoDich);
            var encodedName = Uri.EscapeDataString(_sepay.AccountName);
            QrImageUrl = $"https://img.vietqr.io/image/{bin}-{_sepay.BankNumber}-compact2.jpg?amount=10000&addInfo={encodedInfo}&accountName={encodedName}";
            ShowQr = true;
        }

        return Page();
    }
}
