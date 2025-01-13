namespace Ordering.Application.Order.Queries.GetOrdersByCustomer
{
    public class GetOrdersByCustomerHandler(IApplicationDbContext _dbContext)
        : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
    {
        public async Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
        {
            var orders = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Where(o => o.CustomerId == CustomerId.Of(query.CustomerId))
                .OrderBy(o => o.OrderName)
                .ToListAsync(cancellationToken);

            return new GetOrdersByCustomerResult(orders.ToOrderDtoList());
        }
    }
}
