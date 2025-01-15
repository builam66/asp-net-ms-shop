using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket
{
    public record CheckoutBasketCommand(BasketCheckoutDTO BasketCheckoutDto) : ICommand<CheckoutBasketResult>;

    public record CheckoutBasketResult(bool IsSuccess);

    public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
    {
        public CheckoutBasketCommandValidator()
        {
            RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("Basket data cannot null");
            RuleFor(x => x.BasketCheckoutDto.Username).NotEmpty().WithMessage("Username is required");
        }
    }

    public class CheckoutBasketCommandHandler
        (IBasketRepository basketRepository, IPublishEndpoint publishEndpoint)
        : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasket(command.BasketCheckoutDto.Username, cancellationToken);
            if (basket == null)
            {
                return new CheckoutBasketResult(false);
            }

            var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
            eventMessage.TotalPrice = basket.TotalPrice;

            await publishEndpoint.Publish(eventMessage, cancellationToken);

            await basketRepository.DeleteBasket(command.BasketCheckoutDto.Username, cancellationToken);

            return new CheckoutBasketResult(true);
        }
    }
}
