using MassTransit;

namespace Ordering.Application.Order.EventHandlers.Domain
{
    public class OrderCreatedEventHandler
        (IPublishEndpoint publishEndpoint, IFeatureManager featureManager, ILogger<OrderCreatedEventHandler> logger)
        : INotificationHandler<OrderCreatedEvent>
    {
        public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain Event handled: {DomainEvent}", domainEvent.GetType().Name);

            var isEnabledOrderFullfilment = await featureManager.IsEnabledAsync("OrderFullfilment");
            if (isEnabledOrderFullfilment)
            {
                var orderCreatedIntegrationEvent = domainEvent.Order.ToOrderDto();
                await publishEndpoint.Publish(orderCreatedIntegrationEvent, cancellationToken);
            } 
        }
    }
}
