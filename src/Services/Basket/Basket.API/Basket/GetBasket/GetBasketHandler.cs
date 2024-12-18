namespace Basket.API.Basket.GetBasket
{
    public record GetBasketQuery(string Username) : IQuery<GetBasketResult>;

    public record GetBasketResult(ShoppingCart ShoppingCart);

    public class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
    {
        public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
        {
            // var basket = await _repository.GetBasket(query.Username);
            return new GetBasketResult(new ShoppingCart("builam66"));
        }
    }
}
