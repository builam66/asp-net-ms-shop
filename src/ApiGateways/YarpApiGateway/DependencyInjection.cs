using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
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
                    RouteId = "catalog-public-route",
                    ClusterId = "catalog-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/catalog-service/{**catch-all}",
                        Methods = ["GET"], // Apply for GET only
                    },
                    Transforms = commonTransforms,
                },
                new RouteConfig
                {
                    RouteId = "catalog-admin-route",
                    ClusterId = "catalog-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/catalog-service/{**catch-all}",
                        Methods = ["POST", "PUT", "DELETE"],
                    },
                    Transforms = commonTransforms,
                    AuthorizationPolicy = "AdminPolicy",
                },
                new RouteConfig
                {
                    RouteId = "basket-user-route",
                    ClusterId = "basket-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/basket-service/{**catch-all}"
                    },
                    Transforms = commonTransforms,
                    AuthorizationPolicy = "UserPolicy",
                },
                new RouteConfig
                {
                    RouteId = "ordering-user-route",
                    ClusterId = "ordering-cluster",
                    Match = new RouteMatch
                    {
                        Path = "/ordering-service/{**catch-all}"
                    },
                    Transforms = commonTransforms,
                    AuthorizationPolicy = "UserPolicy",
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
                .AddTransforms<ModifyBodyTransformProvider>()
                .AddTransforms(builderContext =>
                {
                    // Pass the token from the gateway to the downstream services
                    builderContext.AddRequestTransform(async transformContext =>
                    {
                        var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
                        if (string.IsNullOrEmpty(accessToken) == false)
                        {
                            transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        }
                    });
                });

            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = configuration["ClustersAddress:IdentityCluster"]; // Identity server URL
                    options.Audience = "yarp_gateway_resource"; // Must match the audience in Identity server
                    options.RequireHttpsMetadata = false; // Using HTTP
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        // Map the "role" claim from the JWT
                        RoleClaimType = ClaimTypes.Role,
                    };
                });

            services
                .AddAuthorizationBuilder()
                .AddPolicy("AdminPolicy", policy =>
                {
                    policy
                        .RequireAuthenticatedUser()
                        .RequireRole("Admin"); // Requires "Admin" role
                    //policy.RequireClaim("groups", "Admin"); // If roles are stored in a different claim
                })
                .AddPolicy("UserPolicy", policy =>
                {
                    policy
                        .RequireAuthenticatedUser()
                        .RequireRole("User"); // Requires "User" role
                });

            return services;
        }
    }
}
