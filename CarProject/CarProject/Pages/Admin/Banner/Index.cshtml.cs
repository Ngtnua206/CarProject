using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.Banner;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public List<QuangCaoBanner> BannerList { get; set; }

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGetAsync()
    {
        BannerList = await _db.QuangCaoBanner.OrderBy(b => b.ThuTuHienThi).ToListAsync();
    }
}
