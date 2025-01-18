namespace EShop.Web.Pages
{
    public class CartModel
        (IBasketService _basketService, ILogger<CartModel> _logger)
        : PageModel
    {
        public ShoppingCartModel ShoppingCart { get; set; } = new ShoppingCartModel();

        public async Task<IActionResult> OnGetAsync()
        {
            ShoppingCart = await _basketService.LoadUserBasket();

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(Guid productId)
        {
            _logger.LogInformation("Remove to cart button clicked");

            ShoppingCart = await _basketService.LoadUserBasket();
            ShoppingCart.Items.RemoveAll(x => x.ProductId == productId);

            await _basketService.StoreBasket(new StoreBasketRequest(ShoppingCart));

            return RedirectToPage();
        }
    }
}
