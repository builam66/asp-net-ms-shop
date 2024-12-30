namespace Ordering.Domain.Abstractions
{
    public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        IReadOnlyList<IDomainEvent> IAggregate.DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        IDomainEvent[] IAggregate.ClearDomainEvents()
        {
            IDomainEvent[] dequeuedEvents = _domainEvents.ToArray();

            _domainEvents.Clear();

            return dequeuedEvents;
        }
    }
}
