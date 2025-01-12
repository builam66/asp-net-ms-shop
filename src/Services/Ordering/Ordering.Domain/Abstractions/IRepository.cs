namespace Ordering.Domain.Abstractions
{
    public interface IRepository<T> where T : IAggregate
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
