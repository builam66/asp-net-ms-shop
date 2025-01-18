namespace EShop.Web.Pages
{
    public class CheckoutModel
        (IBasketService _basketService, ILogger<CheckoutModel> _logger) 
        : PageModel
    {
        [BindProperty]
        public BasketCheckoutModel Order { get; set; } = default!;

        public ShoppingCartModel Cart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = await _basketService.LoadUserBasket();

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            _logger.LogInformation("Checkout button clicked");

            Cart = await _basketService.LoadUserBasket();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // assumption customerId is passed in from the UI authenticated user        
            Order.CustomerId = new Guid("58c49479-ec65-4de2-86e7-033c546291aa");
            Order.Username = Cart.Username;
            Order.TotalPrice = Cart.TotalPrice;

            await _basketService.CheckoutBasket(new CheckoutBasketRequest(Order));

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}
