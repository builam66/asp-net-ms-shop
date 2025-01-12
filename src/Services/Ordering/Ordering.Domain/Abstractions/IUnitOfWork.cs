namespace Ordering.Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
