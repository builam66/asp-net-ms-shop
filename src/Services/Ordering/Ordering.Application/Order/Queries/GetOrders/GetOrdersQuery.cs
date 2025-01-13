namespace Ordering.Application.Order.Queries.GetOrders
{
    public record GetOrdersQuery(PaginationRequest PaginationRequest) : IQuery<GetOrdersResult>;

    public record GetOrdersResult(PaginatedResult<OrderDto> Orders);
}
