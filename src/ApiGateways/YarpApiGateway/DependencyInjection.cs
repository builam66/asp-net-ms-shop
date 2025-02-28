using Yarp.ReverseProxy.Configuration;
using YarpApiGateway.Helpers;

namespace YarpApiGateway
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYarpReverseProxy(this IServiceCollection services, IConfiguration configuration)
        {
            var commonTransforms = new[]
            {
                new Dictionary<string, string> { { "PathPattern", "{**catch-all}" } }
            };

            var routes = new[]
            {
                new RouteConfig
                {
                    RouteId = "catalog-route",
                    ClusterId = "catalog-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/catalog-service/{**catch-all}"
                    },
                    Transforms = commonTransforms,
                },
                new RouteConfig
                {
                    RouteId = "basket-route",
                    ClusterId = "basket-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/basket-service/{**catch-all}"
                    },
                    Transforms = commonTransforms,
                },
                new RouteConfig
                {
                    RouteId = "ordering-route",
                    ClusterId = "ordering-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/ordering-service/{**catch-all}"
                    },
                    Transforms = commonTransforms,
                },
                new RouteConfig
                {
                    RouteId = "login-route",
                    ClusterId = "identity-cluster",
                    Match = new RouteMatch { Path = "/identity-service/login" },
                    Transforms =
                    [
                        new Dictionary<string, string> { { "PathPattern", "/connect/token" } },
                        //new Dictionary<string, string> { { "RequestTransform", "ModifyLoginRequest" } }
                    ],
                },
                new RouteConfig
                {
                    RouteId = "register-route",
                    ClusterId = "identity-cluster",
                    Match = new RouteMatch { Path = "/identity-service/register" },
                    Transforms =
                    [
                        new Dictionary<string, string> { { "PathPattern", "/connect/user/register" } }
                    ],
                },
            };

            var clusters = new[]
            {
                new ClusterConfig
                {
                    ClusterId = "catalog-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "catalog-service", new DestinationConfig
                            {
                                Address = configuration["ClustersAddress:CatalogCluster"]!,
                            }
                        }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "basket-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "basket-service", new DestinationConfig
                            {
                                Address = configuration["ClustersAddress:BasketCluster"]!,
                            }
                        }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "ordering-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "ordering-service", new DestinationConfig
                            {
                                Address = configuration["ClustersAddress:OrderingCluster"]!,
                            }
                        }
                    }
                },
                new ClusterConfig
                {
                    ClusterId = "identity-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "identity-service", new DestinationConfig
                            {
                                Address = configuration["ClustersAddress:IdentityCluster"]!,
                            }
                        }
                    }
                },
            };

            services
                .AddReverseProxy()
                .LoadFromMemory(routes, clusters)
                .AddTransforms<ModifyBodyTransformProvider>();

            return services;
        }
    }
}
