using System.Net;

namespace EShop.Web.Services
{
    public interface IBasketService
    {
        [Get("/basket-service/basket/{username}")]
        Task<GetBasketResponse> GetBasket(string username);

        [Post("/basket-service/basket")]
        Task<StoreBasketResponse> StoreBasket(StoreBasketRequest request);

        [Delete("/basket-service/basket/{username}")]
        Task<DeleteBasketResponse> DeleteBasket(string username);

        [Post("/basket-service/basket/checkout")]
        Task<CheckoutBasketResponse> CheckoutBasket(CheckoutBasketRequest request);

        public async Task<ShoppingCartModel> LoadUserBasket()
        {
            var username = "builam66";
            ShoppingCartModel shoppingCart;

            try
            {
                var getBasketResponse = await GetBasket(username);
                shoppingCart = getBasketResponse.ShoppingCart;
            }
            catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
            {
                shoppingCart = new ShoppingCartModel
                {
                    Username = username,
                    Items = [],
                };
            }

            return shoppingCart;
        }
    }
}
