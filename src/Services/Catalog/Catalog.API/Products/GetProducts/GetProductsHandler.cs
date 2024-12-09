namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery(int? PageNumber = QueryConstants.DEFAULT_PAGE_NUMBER, int? PageSize = QueryConstants.DEFAULT_PAGE_SIZE)
        : IQuery<GetProductsResult>;

    public record GetProductsResult(IEnumerable<Product> Products, long TotalItemCount);

    internal class GetProductsQueryHandler
        (IDocumentSession session)
        : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>().ToPagedListAsync(
                query.PageNumber ?? QueryConstants.DEFAULT_PAGE_NUMBER,
                query.PageSize ?? QueryConstants.DEFAULT_PAGE_SIZE,
                cancellationToken);

            return new GetProductsResult(products, products.TotalItemCount);
        }
    }
}
