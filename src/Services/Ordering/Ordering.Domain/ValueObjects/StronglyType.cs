namespace Ordering.Domain.ValueObjects
{
    public record OrderId { public Guid Value { get; } }

    public record CustomerId { public Guid Value { get; } }

    public record OrderItemId { public Guid Value { get; } }

    public record ProductId { public Guid Value { get; } }
}
