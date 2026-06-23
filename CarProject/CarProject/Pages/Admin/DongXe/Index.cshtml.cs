using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CarProject.Data;
using CarProject.Models;

namespace CarProject.Pages.Admin.DongXe;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public List<Models.DongXe> DongXeList { get; set; }

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task OnGetAsync()
    {
        DongXeList = await _db.DongXe.Include(d => d.HangXe).ToListAsync();
    }
}
