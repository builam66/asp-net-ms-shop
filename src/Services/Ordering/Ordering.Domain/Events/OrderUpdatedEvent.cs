using Ordering.Domain.Models.Order;

namespace Ordering.Domain.Events
{
    public record OrderUpdatedEvent(Order Order) : IDomainEvent;
}
