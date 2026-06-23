using CarProject.Data;
using CarProject.Models;

namespace CarProject.Services;

public interface IActivityLogService
{
    Task LogAsync(string hanhDong, string chiTiet = null);
}

public class ActivityLogService : IActivityLogService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _http;

    public ActivityLogService(AppDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    public async Task LogAsync(string hanhDong, string chiTiet = null)
    {
        var ctx = _http.HttpContext;
        var entry = new NhatKyHeThong
        {
            MaTaiKhoan = ctx?.Session.GetInt32("UserId"),
            TenDangNhap = ctx?.Session.GetString("UserName") ?? "(anonymous)",
            VaiTro = ctx?.Session.GetString("UserRole") ?? "Guest",
            HanhDong = hanhDong,
            ChiTiet = chiTiet ?? "",
            DiaChiIP = ctx?.Connection.RemoteIpAddress?.ToString(),
            TrinhDuyet = ctx?.Request.Headers.UserAgent.ToString(),
            DuongDan = ctx?.Request.Path,
            ThoiGian = DateTime.UtcNow
        };
        _db.NhatKyHeThong.Add(entry);
        await _db.SaveChangesAsync();
    }
}
