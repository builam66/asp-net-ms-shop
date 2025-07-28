namespace Ordering.Domain.Models.Order
{
    /// <summary>
    /// Repository interface for managing <see cref="Order"/> entities.
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Adds a new order to the repository.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <returns>The added order entity.</returns>
        Order Add(Order order);

        /// <summary>
        /// Updates an existing order in the repository.
        /// </summary>
        /// <param name="order">The order to update.</param>
        void Update(Order order);

        /// <summary>
        /// Retrieves an order by its unique identifier.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The order entity if found; otherwise, null.</returns>
        Task<Order> GetAsync(Guid orderId, CancellationToken cancellationToken);
    }
}
