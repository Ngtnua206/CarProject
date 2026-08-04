using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;
using CarProject.Services;

namespace CarProject.Pages.QuanLy;

public class LichHenModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IActivityLogService _log;
    private readonly INotificationService _notif;

    public LichHenModel(AppDbContext db, IActivityLogService log, INotificationService notif)
    {
        _db = db;
        _log = log;
        _notif = notif;
    }

    public ChiNhanhShowroom? Showroom { get; set; }
    public List<LichHenLaiThu> DanhSachLichHen { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName))
            return RedirectToPage("/Account/Login");

        Showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (Showroom == null)
        {
            ErrorMessage = "Bạn chưa được phân công quản lý showroom nào.";
            return Page();
        }

        DanhSachLichHen = await _db.LichHenLaiThu
            .Include(l => l.DongXe)
            .Include(l => l.KhachHang)
            .Where(l => l.MaChiNhanh == Showroom.MaChiNhanh)
            .OrderByDescending(l => l.NgayHen)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAcceptAsync(int maLichHen)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        Showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (Showroom == null)
        {
            ErrorMessage = "Bạn chưa được phân công quản lý showroom nào.";
            return RedirectToPage();
        }

        var lichHen = await _db.LichHenLaiThu.FindAsync(maLichHen);
        if (lichHen == null) return NotFound();
        if (lichHen.MaChiNhanh != Showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền duyệt lịch hẹn này.";
            return RedirectToPage();
        }

        var wasPending = lichHen.TrangThai == "Chờ xác nhận";
        lichHen.TrangThai = "Đã xác nhận";
        await _db.SaveChangesAsync();

        if (wasPending)
        {
            await CapNhatDoanhThuLaiThuAsync(lichHen);
        }

        await _notif.SendAsync(lichHen.MaKhachHang, "Lịch lái thử đã được xác nhận",
            $"Lịch lái thử xe vào {lichHen.NgayHen:dd/MM/yyyy} lúc {lichHen.GioHen} đã được xác nhận.", "/TestDrive");
        await _log.LogAsync($"QL duyệt lịch hẹn #{maLichHen}");

        SuccessMessage = "Đã xác nhận lịch hẹn.";
        return RedirectToPage();
    }

    private async Task CapNhatDoanhThuLaiThuAsync(LichHenLaiThu lichHen)
    {
        const long bookingRevenue = 1_000_000;
        var kyBaoCao = DateTime.Now.ToString("yyyy-MM");

        var thongKe = await _db.ThongKeTongHop_Boss
            .FirstOrDefaultAsync(t => t.KyBaoCao == kyBaoCao && t.MaChiNhanh == lichHen.MaChiNhanh);

        if (thongKe == null)
        {
            thongKe = new ThongKeTongHop_Boss
            {
                KyBaoCao = kyBaoCao,
                MaChiNhanh = lichHen.MaChiNhanh,
                TongDoanhThu = bookingRevenue,
                TongTienCocThuVe = 0,
                TongSoXeDaBan = 0,
                SoDonCocBiHuy = 0,
                TongLuotXemWeb = 0,
                TongLuotLaiThu = 1,
                MaDongXeBanChayNhat = lichHen.MaDong
            };
            _db.ThongKeTongHop_Boss.Add(thongKe);
        }
        else
        {
            thongKe.TongDoanhThu += bookingRevenue;
            thongKe.TongLuotLaiThu += 1;
            if (thongKe.MaDongXeBanChayNhat == 0)
            {
                thongKe.MaDongXeBanChayNhat = lichHen.MaDong;
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IActionResult> OnPostRejectAsync(int maLichHen)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        Showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (Showroom == null)
        {
            ErrorMessage = "Bạn chưa được phân công quản lý showroom nào.";
            return RedirectToPage();
        }

        var lichHen = await _db.LichHenLaiThu.FindAsync(maLichHen);
        if (lichHen == null) return NotFound();
        if (lichHen.MaChiNhanh != Showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền xử lý lịch hẹn này.";
            return RedirectToPage();
        }

        lichHen.TrangThai = "Từ chối";
        await _db.SaveChangesAsync();

        await _notif.SendAsync(lichHen.MaKhachHang, "Lịch lái thử bị từ chối",
            $"Lịch lái thử xe vào {lichHen.NgayHen:dd/MM/yyyy} lúc {lichHen.GioHen} đã bị từ chối. Vui lòng liên hệ showroom để biết thêm chi tiết.", "/TestDrive");
        await _log.LogAsync($"QL từ chối lịch hẹn #{maLichHen}");

        SuccessMessage = "Đã từ chối lịch hẹn.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int maLichHen)
    {
        var userName = User.GetJwtUserName();
        if (string.IsNullOrEmpty(userName)) return RedirectToPage("/Account/Login");

        Showroom = await _db.ChiNhanhShowroom.FirstOrDefaultAsync(c => c.MaQuanLy == userName);
        if (Showroom == null)
        {
            ErrorMessage = "Bạn chưa được phân công quản lý showroom nào.";
            return RedirectToPage();
        }

        var lichHen = await _db.LichHenLaiThu.FindAsync(maLichHen);
        if (lichHen == null) return NotFound();
        if (lichHen.MaChiNhanh != Showroom.MaChiNhanh)
        {
            ErrorMessage = "Bạn không có quyền xử lý lịch hẹn này.";
            return RedirectToPage();
        }

        lichHen.TrangThai = "Hoàn thành";
        await _db.SaveChangesAsync();

        await _log.LogAsync($"QL hoàn thành lịch hẹn #{maLichHen}");

        SuccessMessage = "Đã đánh dấu hoàn thành.";
        return RedirectToPage();
    }
}