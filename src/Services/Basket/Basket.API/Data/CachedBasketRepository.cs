using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data
{
    public class CachedBasketRepository(IBasketRepository basketRepository, IDistributedCache distributedCache)
        : IBasketRepository
    {
        public async Task<bool> DeleteBasket(string username, CancellationToken cancellationToken = default)
        {
            await basketRepository.DeleteBasket(username, cancellationToken);

            await distributedCache.RemoveAsync(username, cancellationToken);

            return true;
        }

        public async Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken = default)
        {
            var cachedBasket = await distributedCache.GetStringAsync(username, cancellationToken);

            if (!string.IsNullOrEmpty(cachedBasket))
            {
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
            }

            var basket = await basketRepository.GetBasket(username, cancellationToken);
            await distributedCache.SetStringAsync(username, JsonSerializer.Serialize(basket), cancellationToken);

            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            await basketRepository.StoreBasket(shoppingCart, cancellationToken);

            await distributedCache.SetStringAsync(shoppingCart.Username, JsonSerializer.Serialize(shoppingCart), cancellationToken);

            return shoppingCart;
        }
    }
}
