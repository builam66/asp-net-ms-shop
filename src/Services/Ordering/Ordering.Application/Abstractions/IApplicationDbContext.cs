namespace Ordering.Application.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; }

        DbSet<Product> Products { get; }

        DbSet<Domain.Models.Order.Order> Orders { get; }

        DbSet<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
