namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart ShoppingCart) : ICommand<StoreBasketResult>;

    public record StoreBasketResult(string Username);

    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.ShoppingCart).NotNull().WithMessage("Cart cannot be null");
            RuleFor(x => x.ShoppingCart.Username).NotEmpty().WithMessage("Username is required");
        }
    }

    public class StoreBasketCommandHandler
        (IBasketRepository basketRepository, DiscountProtoService.DiscountProtoServiceClient discountProto)
        : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            await CalculateDiscount(command.ShoppingCart, cancellationToken);

            var result = await basketRepository.StoreBasket(command.ShoppingCart, cancellationToken);

            return new StoreBasketResult(result.Username);
        }

        private async Task CalculateDiscount(ShoppingCart shoppingCart, CancellationToken cancellationToken)
        {
            foreach (var item in shoppingCart.Items)
            {
                var coupon = await discountProto.GetDiscountAsync(
                    new GetDiscountRequest { ProductName = item.ProductName },
                    cancellationToken: cancellationToken);

                item.Price -= coupon.Amount;
            }
        }
    }
}
