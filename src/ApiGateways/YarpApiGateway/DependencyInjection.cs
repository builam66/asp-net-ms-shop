using Yarp.ReverseProxy.Configuration;

namespace YarpApiGateway
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYarpReverseProxy(this IServiceCollection services, IConfiguration configuration)
        {
            var transforms = new[]
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
                    Transforms = transforms,
                },
                new RouteConfig
                {
                    RouteId = "basket-route",
                    ClusterId = "basket-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/basket-service/{**catch-all}"
                    },
                    Transforms = transforms,
                },
                new RouteConfig
                {
                    RouteId = "ordering-route",
                    ClusterId = "ordering-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/ordering-service/{**catch-all}"
                    },
                    Transforms = transforms,
                }
            };

            var clusters = new[]
            {
                new ClusterConfig
                {
                    ClusterId = "catalog-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "destination1", new DestinationConfig
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
                            "destination1", new DestinationConfig
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
                            "destination1", new DestinationConfig
                            {
                                Address = configuration["ClustersAddress:OrderingCluster"]!,
                            }
                        }
                    }
                }
            };

            services.AddReverseProxy().LoadFromMemory(routes, clusters);

            return services;
        }
    }
}
