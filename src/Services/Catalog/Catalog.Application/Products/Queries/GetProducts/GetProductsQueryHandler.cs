using BuildingBlocks.CQRS;
using BuildingBlocks.Common;
using BuildingBlocks.Pagination;
using Catalog.Domain.Repositories;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler(
    IProductRepository productRepository,
    ILogger<GetProductsQueryHandler> logger)
    : IQueryHandler<GetProductsQuery, Result<PaginatedResult<ProductDto>>>
{
    public async Task<Result<PaginatedResult<ProductDto>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting products with page {PageNumber}, size {PageSize}, category {Category}, search {SearchTerm}",
            query.PageNumber, query.PageSize, query.Category, query.SearchTerm);

        try
        {
            var products = await GetFilteredProducts(query, cancellationToken);
            var totalCount = await GetTotalCount(query, cancellationToken);

            var productDtos = products.Adapt<IEnumerable<ProductDto>>();

            var paginatedResult = new PaginatedResult<ProductDto>(
                productDtos,
                totalCount,
                query.PageNumber,
                query.PageSize
            );

            logger.LogInformation("Successfully retrieved {Count} products", productDtos.Count());

            return Result.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving products");
            return Result.Failure<PaginatedResult<ProductDto>>("An error occurred while retrieving products");
        }
    }

    private async Task<IEnumerable<Catalog.Domain.Entities.Product>> GetFilteredProducts(GetProductsQuery query, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            return await productRepository.SearchProductsAsync(query.SearchTerm, cancellationToken);
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            return await productRepository.GetProductsByCategoryAsync(query.Category, cancellationToken);
        }

        return await productRepository.GetActiveProductsAsync(cancellationToken);
    }

    private async Task<int> GetTotalCount(GetProductsQuery query, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            var searchResults = await productRepository.SearchProductsAsync(query.SearchTerm, cancellationToken);
            return searchResults.Count();
        }

        if (!string.IsNullOrEmpty(query.Category))
        {
            var categoryResults = await productRepository.GetProductsByCategoryAsync(query.Category, cancellationToken);
            return categoryResults.Count();
        }

        return await productRepository.CountAsync(p => p.IsActive, cancellationToken);
    }
}