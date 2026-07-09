using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarProject.Pages.Admin;

public class ToggleModeModel : PageModel
{
    public IActionResult OnGet(string mode, string? returnUrl)
    {
        if (mode == "View" || mode == "Control")
        {
            HttpContext.Session.SetString("AdminMode", mode);
        }

        var redirectUrl = returnUrl ?? "/";
        return Redirect(redirectUrl);
    }
}
