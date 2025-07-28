namespace Ordering.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="Order"/> entities using Entity Framework Core.
    /// </summary>
    public class OrderRepository(ApplicationDbContext dbContext)
        : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        /// <summary>
        /// Gets the unit of work for the repository.
        /// </summary>
        public IUnitOfWork UnitOfWork => _dbContext;

        /// <summary>
        /// Adds a new order to the database context.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <returns>The added order entity.</returns>
        public Order Add(Order order)
        {
            return _dbContext.Orders.Add(order).Entity;
        }

        /// <summary>
        /// Retrieves an order by its unique identifier, including its order items.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The order entity if found; otherwise, null.</returns>
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

        /// <summary>
        /// Updates an existing order in the database context.
        /// </summary>
        /// <param name="order">The order to update.</param>
        public void Update(Order order)
        {
            _dbContext.Entry(order).State = EntityState.Modified;
        }
    }
}
