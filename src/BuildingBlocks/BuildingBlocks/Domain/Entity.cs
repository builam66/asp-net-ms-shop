using MediatR;

namespace BuildingBlocks.Domain;

public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id!.Equals(default(TId)) || other.Id!.Equals(default(TId)))
            return false;

        return Id.Equals(other.Id);
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
{
    private readonly List<INotification> _domainEvents = new();

    public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(INotification domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public abstract class AuditableEntity<TId> : AggregateRoot<TId>, IAuditableEntity
{
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public interface IEntity<TId>
{
    TId Id { get; }
}

public interface IAggregateRoot<TId> : IEntity<TId>
{
    IReadOnlyList<INotification> DomainEvents { get; }
    void AddDomainEvent(INotification domainEvent);
    void RemoveDomainEvent(INotification domainEvent);
    void ClearDomainEvents();
}

public interface IAuditableEntity
{
    string? CreatedBy { get; set; }
    DateTime CreatedAt { get; set; }
    string? LastModifiedBy { get; set; }
    DateTime? LastModifiedAt { get; set; }
}