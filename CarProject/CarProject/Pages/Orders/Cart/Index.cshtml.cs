using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Services;
using CarProject.Data;
using Microsoft.EntityFrameworkCore;

namespace CarProject.Pages.Orders.Cart;

public class IndexModel : PageModel
{
    private readonly ICartService _cart;
    private readonly AppDbContext _db;

    public List<CartItem> CartItems { get; set; } = new();
    public decimal TotalDeposit { get; set; }
    public int ItemCount { get; set; }

    public IndexModel(ICartService cart, AppDbContext db)
    {
        _cart = cart;
        _db = db;
    }

    public async Task OnGetAsync()
    {
        CartItems = await _cart.GetCartAsync();
        ItemCount = CartItems.Sum(c => c.SoLuong);
        TotalDeposit = await _cart.GetTotalDepositAsync();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int maPhienBan)
    {
        await _cart.RemoveFromCartAsync(maPhienBan);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int maPhienBan, int soLuong)
    {
        var phienBan = await _db.PhienBanXe.FindAsync(maPhienBan);
        if (phienBan != null && soLuong > phienBan.SoLuongTrongKho)
        {
            TempData["CartError"] = $"Xe {(phienBan.TenPhienBan ?? "")} chỉ còn {phienBan.SoLuongTrongKho} xe trong kho.";
            return RedirectToPage();
        }
        await _cart.UpdateQuantityAsync(maPhienBan, soLuong);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostClearAsync()
    {
        await _cart.ClearCartAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        var items = await _cart.GetCartAsync();
        var totalItems = items.Sum(c => c.SoLuong);
        if (totalItems < 3)
        {
            TempData["CartError"] = "Cần ít nhất 3 xe để tiến hành đặt cọc theo giỏ hàng.";
            return RedirectToPage();
        }
        return RedirectToPage("/Orders/Cart/Checkout");
    }
}
