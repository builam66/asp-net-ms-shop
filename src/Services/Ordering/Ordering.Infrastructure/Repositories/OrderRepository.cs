namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository(ApplicationDbContext dbContext)
        : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public IUnitOfWork UnitOfWork => _dbContext;

        public Order Add(Order order)
        {
            return _dbContext.Orders.Add(order).Entity;
        }

        public async Task<Order> GetAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var stronglyOrderId = OrderId.Of(orderId);
            var order = await _dbContext.Orders.FindAsync([stronglyOrderId], cancellationToken);

            if (order != null)
            {
                await _dbContext.Entry(order)
                    .Collection(i => i.OrderItems)
                    .LoadAsync(cancellationToken);
            }

            return order!;
        }

        public void Update(Order order)
        {
            _dbContext.Entry(order).State = EntityState.Modified;
        }
    }
}
