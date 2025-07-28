using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Abstractions;
using Ordering.Infrastructure.Interceptors;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Infrastructure
{
    /// <summary>
    /// Provides extension methods for registering infrastructure layer services for the Ordering service.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers infrastructure services such as DbContext, interceptors, and repositories.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
