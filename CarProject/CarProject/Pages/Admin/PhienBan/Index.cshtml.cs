using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.PhienBan;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public List<PhienBanXe> PhienBanList { get; set; }

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGetAsync()
    {
        PhienBanList = await _db.PhienBanXe.Include(p => p.DongXe).ToListAsync();
    }
}
