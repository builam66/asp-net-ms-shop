using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Ordering.API
{
    /// <summary>
    /// Provides extension methods for registering API layer services and middleware for the Ordering service.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers API-specific services such as Carter, exception handling, and health checks.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCarter();

            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("Database")!);

            return services;
        }

        /// <summary>
        /// Configures API-specific middleware such as Carter endpoints, exception handling, and health checks.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <returns>The updated web application instance.</returns>
        public static WebApplication UseAPIServices(this WebApplication app)
        {
            app.MapCarter();

            app.UseExceptionHandler(options => { });
            app.UseHealthChecks("/health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            return app;
        }
    }
}
