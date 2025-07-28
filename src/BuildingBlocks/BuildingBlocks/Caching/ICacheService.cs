namespace BuildingBlocks.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}

public static class CacheKeys
{
    private const string Separator = ":";
    
    public static class Products
    {
        private const string Prefix = "products";
        public static string GetAll => $"{Prefix}{Separator}all";
        public static string GetById(Guid id) => $"{Prefix}{Separator}id{Separator}{id}";
        public static string GetByCategory(string category) => $"{Prefix}{Separator}category{Separator}{category}";
        public static string Search(string term) => $"{Prefix}{Separator}search{Separator}{term}";
    }
    
    public static class Orders
    {
        private const string Prefix = "orders";
        public static string GetByCustomer(string customerId) => $"{Prefix}{Separator}customer{Separator}{customerId}";
        public static string GetById(Guid id) => $"{Prefix}{Separator}id{Separator}{id}";
    }
    
    public static class Baskets
    {
        private const string Prefix = "baskets";
        public static string GetByUser(string userId) => $"{Prefix}{Separator}user{Separator}{userId}";
    }
}