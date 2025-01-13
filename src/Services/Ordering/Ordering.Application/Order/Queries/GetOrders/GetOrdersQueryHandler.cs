namespace Ordering.Application.Order.Queries.GetOrders
{
    public class GetOrdersHandler(IApplicationDbContext _dbContext)
        : IQueryHandler<GetOrdersQuery, GetOrdersResult>
    {
        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCount = await _dbContext.Orders.LongCountAsync(cancellationToken);

            var orders = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .OrderBy(o => o.OrderName)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new GetOrdersResult(
                new PaginatedResult<OrderDto>(
                    pageIndex,
                    pageSize,
                    totalCount,
                    orders.ToOrderDtoList()));
        }
    }
}
