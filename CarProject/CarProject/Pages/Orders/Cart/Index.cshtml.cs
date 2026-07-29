using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarProject.Services;

namespace CarProject.Pages.Orders.Cart;

public class IndexModel : PageModel
{
    private readonly ICartService _cart;

    public List<CartItem> CartItems { get; set; } = new();
    public decimal TotalDeposit { get; set; }
    public int ItemCount { get; set; }

    public IndexModel(ICartService cart)
    {
        _cart = cart;
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
