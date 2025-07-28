using BuildingBlocks.CQRS;
using BuildingBlocks.Common;
using BuildingBlocks.Pagination;

namespace Catalog.Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Category = null,
    string? SearchTerm = null
) : IQuery<Result<PaginatedResult<ProductDto>>>;

public record ProductDto(
    Guid Id,
    string Name,
    List<string> Categories,
    string Description,
    string ImageUri,
    decimal Price,
    int Stock,
    bool IsActive
);