namespace EShop.Web.Models.Ordering
{
    public class PaginatedModel<TEntity>
        (int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        where TEntity : class
    {
        public int PageIndex { get; } = pageIndex;

        public int PageSize { get; } = pageSize;

        public long Count { get; } = count;

        public IEnumerable<TEntity> Data { get; } = data;
    }
}
