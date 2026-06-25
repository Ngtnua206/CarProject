using CarProject.Data;
using CarProject.Models;
using Serilog;

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
        var userName = ctx?.Session.GetString("UserName") ?? "(anonymous)";
        var role = ctx?.Session.GetString("UserRole") ?? "Guest";
        var ip = ctx?.Connection.RemoteIpAddress?.ToString();
        var path = ctx?.Request.Path;

        // Ghi ra file log Serilog (không cần DB)
        Log.Information("[{Role}] {User} | {Action} | {Detail} | {Path} | {IP}",
            role, userName, hanhDong, chiTiet, path, ip);

        // Ghi vào DB nếu có quyền (bỏ qua lỗi nếu không)
        try
        {
            var entry = new NhatKyHeThong
            {
                MaTaiKhoan = ctx?.Session.GetInt32("UserId"),
                TenDangNhap = userName,
                VaiTro = role,
                HanhDong = hanhDong,
                ChiTiet = chiTiet ?? "",
                DiaChiIP = ip,
                TrinhDuyet = ctx?.Request.Headers.UserAgent.ToString(),
                DuongDan = path,
                ThoiGian = DateTime.UtcNow
            };
            _db.NhatKyHeThong.Add(entry);
            await _db.SaveChangesAsync();
        }
        catch
        {
            // ignore DB logging errors
        }
    }
}
