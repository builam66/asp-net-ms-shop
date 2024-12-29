namespace Ordering.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services)
        {
            return services;
        }

        public static WebApplication UseAPIServices(this WebApplication app)
        {
            return app;
        }
    }
}
