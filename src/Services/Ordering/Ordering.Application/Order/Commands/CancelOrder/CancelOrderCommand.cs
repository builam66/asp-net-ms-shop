namespace Ordering.Application.Order.Commands.CancelOrder
{
    public record CancelOrderCommand(Guid OrderId) 
        : ICommand<CancelOrderResult>;

    public record CancelOrderResult(bool IsSuccess);

    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderId is required");
        }
    }
}
