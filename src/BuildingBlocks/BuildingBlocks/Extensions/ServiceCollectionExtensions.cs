using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;

namespace BuildingBlocks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, Assembly assembly)
    {
        // Add MediatR with common behaviors
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        // Add FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Add Exception Handler
        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, string connectionString, string serviceName)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Add database-specific health checks based on connection string
        if (connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase))
        {
            healthChecksBuilder.AddNpgSql(connectionString, name: $"{serviceName}-database");
        }
        else if (connectionString.Contains("sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            healthChecksBuilder.AddSqlServer(connectionString, name: $"{serviceName}-database");
        }

        return services;
    }
}