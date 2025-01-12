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

        public async Task<Order> GetAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public void Update(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
