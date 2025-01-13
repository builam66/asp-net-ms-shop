namespace Ordering.Application.Order.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler(IOrderRepository _orderRepository)
        : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await _orderRepository
                .GetAsync(command.Order.Id, cancellationToken: cancellationToken);

            if (order is null)
            {
                throw new OrderNotFoundException(command.Order.Id);
            }

            var updatedShippingAddress = Address.Of(
                command.Order.ShippingAddress.FirstName,
                command.Order.ShippingAddress.LastName,
                command.Order.ShippingAddress.Email,
                command.Order.ShippingAddress.AddressLine,
                command.Order.ShippingAddress.Country,
                command.Order.ShippingAddress.City,
                command.Order.ShippingAddress.ZipCode);

            var updatedBillingAddress = Address.Of(
                command.Order.BillingAddress.FirstName,
                command.Order.BillingAddress.LastName,
                command.Order.BillingAddress.Email,
                command.Order.BillingAddress.AddressLine,
                command.Order.BillingAddress.Country,
                command.Order.BillingAddress.City,
                command.Order.BillingAddress.ZipCode);

            var updatedPayment = Payment.Of(
                command.Order.Payment.CardName,
                command.Order.Payment.CardNumber,
                command.Order.Payment.Expiration,
                command.Order.Payment.CVV,
                command.Order.Payment.PaymentMethod);

            order.Update(
                orderName: command.Order.OrderName,
                shippingAddress: updatedShippingAddress,
                billingAddress: updatedBillingAddress,
                payment: updatedPayment,
                orderStatus: command.Order.Status);

            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new UpdateOrderResult(true);
        }
    }
}
