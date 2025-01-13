namespace Ordering.Application.Order.Queries.GetOrdersByCustomer
{
    public record GetOrdersByCustomerQuery(Guid CustomerId) : IQuery<GetOrdersByCustomerResult>;

    public record GetOrdersByCustomerResult(IEnumerable<OrderDto> Orders);
}
