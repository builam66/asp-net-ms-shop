namespace Ordering.Application.Order.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(IOrderRepository _orderRepository)
        : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var shippingAddress = Address.Of(
                command.Order.ShippingAddress.FirstName,
                command.Order.ShippingAddress.LastName,
                command.Order.ShippingAddress.Email,
                command.Order.ShippingAddress.AddressLine,
                command.Order.ShippingAddress.Country,
                command.Order.ShippingAddress.City,
                command.Order.ShippingAddress.ZipCode);

            var billingAddress = Address.Of(
                command.Order.BillingAddress.FirstName,
                command.Order.BillingAddress.LastName,
                command.Order.BillingAddress.Email,
                command.Order.BillingAddress.AddressLine,
                command.Order.BillingAddress.Country,
                command.Order.BillingAddress.City,
                command.Order.BillingAddress.ZipCode);

            var newOrder = Domain.Models.Order.Order.Create(
                    id: OrderId.Of(Guid.NewGuid()),
                    customerId: CustomerId.Of(command.Order.CustomerId),
                    orderName: command.Order.OrderName,
                    shippingAddress: shippingAddress,
                    billingAddress: billingAddress,
                    payment: Payment.Of(
                        command.Order.Payment.CardName,
                        command.Order.Payment.CardNumber,
                        command.Order.Payment.Expiration,
                        command.Order.Payment.Cvv,
                        command.Order.Payment.PaymentMethod)
                    );

            foreach (var orderItemDto in command.Order.OrderItems)
            {
                newOrder.Add(ProductId.Of(orderItemDto.ProductId), orderItemDto.Quantity, orderItemDto.Price);
            }

            _orderRepository.Add(newOrder);

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new CreateOrderResult(newOrder.Id.Value);
        }
    }
}
