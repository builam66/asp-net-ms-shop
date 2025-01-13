namespace Ordering.Application.Order.Commands.CancelOrder
{
    public class CancelOrderCommandHandler(IOrderRepository _orderRepository)
        : ICommandHandler<CancelOrderCommand, CancelOrderResult>
    {
        public async Task<CancelOrderResult> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository
                .GetAsync(command.OrderId, cancellationToken: cancellationToken);

            if (order is null)
            {
                throw new OrderNotFoundException(command.OrderId);
            }

            order.SetCancelledStatus();
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new CancelOrderResult(true);
        }
    }
}
