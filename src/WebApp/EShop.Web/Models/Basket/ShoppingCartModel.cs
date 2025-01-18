namespace EShop.Web.Models.Basket
{
    public class ShoppingCartModel
    {
        public string Username { get; set; } = default!;

        public List<ShoppingCartItemModel> Items { get; set; } = [];

        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
    }

    public record GetBasketResponse(ShoppingCartModel ShoppingCart);

    public record StoreBasketRequest(ShoppingCartModel ShoppingCart);

    public record StoreBasketResponse(string Username);

    public record DeleteBasketResponse(bool IsSuccess);
}
