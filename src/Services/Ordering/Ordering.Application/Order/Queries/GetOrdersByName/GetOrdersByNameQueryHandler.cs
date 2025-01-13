namespace Ordering.Application.Order.Queries.GetOrdersByName
{
    public class GetOrdersByNameHandler(IApplicationDbContext _dbContext)
        : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
    {
        public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
        {
            var orders = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Where(o => o.OrderName.Contains(query.Name))
                .OrderBy(o => o.OrderName)
                .ToListAsync(cancellationToken);

            return new GetOrdersByNameResult(orders.ToOrderDtoList());
        }
    }
}
