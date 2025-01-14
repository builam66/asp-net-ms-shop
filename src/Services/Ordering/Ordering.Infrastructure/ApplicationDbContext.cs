using Ordering.Application.Abstractions;
using System.Reflection;

namespace Ordering.Infrastructure
{
    public class ApplicationDbContext : DbContext, IUnitOfWork, IApplicationDbContext
    {
        // Add-Migration InitialMigration -OutputDir Migrations -Project Ordering.Infrastructure -StartupProject Ordering.API
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch domain events before saveChanges
            // in DispatchDomainEventsInterceptor

            await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
