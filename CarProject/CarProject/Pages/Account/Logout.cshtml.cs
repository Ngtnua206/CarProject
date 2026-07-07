using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Services;

namespace CarProject.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly IActivityLogService _log;

    public LogoutModel(IActivityLogService log)
    {
        _log = log;
    }

    public async Task<IActionResult> OnGet()
    {
        var name = HttpContext.Session.GetString("UserName") ?? "(unknown)";
        await _log.LogAsync("Đăng xuất", $"Tài khoản \"{name}\"");
        HttpContext.Session.Clear();
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToPage("/Index");
    }
}
