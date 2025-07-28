using MediatR;

namespace Catalog.Domain.Events;

public record ProductCreatedEvent(Guid ProductId, string ProductName, decimal Price) : INotification;

public record ProductUpdatedEvent(Guid ProductId, string ProductName) : INotification;

public record ProductPriceChangedEvent(Guid ProductId, decimal OldPrice, decimal NewPrice) : INotification;

public record ProductOutOfStockEvent(Guid ProductId, string ProductName) : INotification;

public record ProductBackInStockEvent(Guid ProductId, string ProductName, int Stock) : INotification;

public record ProductDeactivatedEvent(Guid ProductId, string ProductName) : INotification;