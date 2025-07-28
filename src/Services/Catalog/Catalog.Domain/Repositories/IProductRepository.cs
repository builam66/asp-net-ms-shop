using BuildingBlocks.Data;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> IsProductNameUniqueAsync(string name, Guid? excludeProductId = null, CancellationToken cancellationToken = default);
}