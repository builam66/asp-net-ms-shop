using System.Linq.Expressions;
using BuildingBlocks.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> FindAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Product?> FindOneAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Products.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product entity, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Product, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null 
            ? await _context.Products.CountAsync(cancellationToken)
            : await _context.Products.CountAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.Categories.Contains(category) && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.IsActive && 
                       (p.Name.Contains(searchTerm) || 
                        p.Description.Contains(searchTerm) ||
                        p.Categories.Any(c => c.Contains(searchTerm))))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsProductNameUniqueAsync(string name, Guid? excludeProductId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.Where(p => p.Name == name);
        
        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Id != excludeProductId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }
}