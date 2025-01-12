using Ordering.Domain.Models.Order;

namespace Ordering.Domain.Events
{
    public record OrderCreatedEvent(Order Order) : IDomainEvent;
}
