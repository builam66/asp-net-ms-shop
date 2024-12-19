namespace Basket.API.Basket.GetBasket
{
    public record GetBasketQuery(string Username) : IQuery<GetBasketResult>;

    public record GetBasketResult(ShoppingCart ShoppingCart);

    public class GetBasketQueryHandler(IBasketRepository basketRepository)
        : IQueryHandler<GetBasketQuery, GetBasketResult>
    {
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasket(query.Username, cancellationToken);

            return new GetBasketResult(basket);
        }
    }
}
