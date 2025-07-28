namespace Ordering.Domain.Abstractions
{
    /// <summary>
    /// Generic repository interface for aggregate root entities.
    /// </summary>
    public interface IRepository<T> where T : IAggregate
    {
        /// <summary>
        /// Gets the unit of work associated with the repository.
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}
