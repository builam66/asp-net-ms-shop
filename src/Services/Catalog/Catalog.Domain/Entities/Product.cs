using BuildingBlocks.Domain;
using Catalog.Domain.Events;

namespace Catalog.Domain.Entities;

public class Product : AuditableEntity<Guid>
{
    public string Name { get; private set; } = default!;
    public List<string> Categories { get; private set; } = [];
    public string Description { get; private set; } = default!;
    public string ImageUri { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Private constructor for EF Core
    private Product() { }

    public Product(string name, List<string> categories, string description, string imageUri, decimal price, int stock = 0)
    {
        Id = Guid.NewGuid();
        Name = name;
        Categories = categories;
        Description = description;
        ImageUri = imageUri;
        Price = price;
        Stock = stock;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }

    public void UpdateDetails(string name, List<string> categories, string description, string imageUri, decimal price)
    {
        var oldPrice = Price;
        
        Name = name;
        Categories = categories;
        Description = description;
        ImageUri = imageUri;
        Price = price;
        LastModifiedAt = DateTime.UtcNow;

        if (oldPrice != price)
        {
            AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, price));
        }

        AddDomainEvent(new ProductUpdatedEvent(Id, Name));
    }

    public void UpdateStock(int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(newStock));

        var oldStock = Stock;
        Stock = newStock;
        LastModifiedAt = DateTime.UtcNow;

        if (oldStock > 0 && newStock == 0)
        {
            AddDomainEvent(new ProductOutOfStockEvent(Id, Name));
        }
        else if (oldStock == 0 && newStock > 0)
        {
            AddDomainEvent(new ProductBackInStockEvent(Id, Name, newStock));
        }
    }

    public void Activate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductDeactivatedEvent(Id, Name));
    }

    public bool IsInStock() => Stock > 0;
    public bool IsAvailable() => IsActive && IsInStock();
}