namespace Identity.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOpenIddict
            (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Register data storage context
                // Configure the context to use an in-memory store.
                options.UseNpgsql(configuration.GetConnectionString("Database"));
                // Register the entity sets needed by OpenIddict.
                options.UseOpenIddict();
                // Authorization server is integrated with the data storage,
                // ready to receive registration of data related to the authorization of applications
            });

            services.AddOpenIddict()
                // Register the OpenIddict core components
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the EF Core stores/models
                    options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
                })
                // Register the OpenIddict server components
                .AddServer(options =>
                {
                    // AllowClientCredentialsFlow - application (client) needs to authenticate itself, server-to-server, not require user authentication
                    // AllowAuthorizationCodeFlow - obtain an access token of a user, log in to access resources
                    // AllowRefreshTokenFlow - use refresh token to get a new access token
                    // AllowImplicitFlow - client-side applications, no server-side code
                    // AllowHybridFlow - combination of AllowAuthorizationCodeFlow and AllowImplicitFlow (/authorization & /token)
                    // AllowPasswordFlow - directly using the user's username and password
                    options
                        //.AllowClientCredentialsFlow()
                        //.AllowAuthorizationCodeFlow()
                        .AllowRefreshTokenFlow()
                        //.AllowHybridFlow()
                        .AllowPasswordFlow();
                    //.RequireProofKeyForCodeExchange();

                    options
                        //.SetAuthorizationEndpointUris("/connect/authorize")
                        .SetTokenEndpointUris("/connect/token")
                        .SetEndSessionEndpointUris("/connect/logout");

                    // Generate certificates in a development environment
                    options
                        .AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    // Encryption and signing of tokens
                    // AddEncryptionCertificate
                    // AddSigningCertificate
                    options
                        .AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey();

                    // Register scopes (permissions)
                    options.RegisterScopes(Scopes.OpenId, Scopes.Profile, Scopes.OfflineAccess);

                    // Disable access token encryption, simplify token handling
                    options.DisableAccessTokenEncryption();

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    // Enable initial handling of OpenID Connect requests by OpenIddict on endpoints that must be created with the same route as the authentication endpoint been deployed
                    options
                        .UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        //.EnableAuthorizationEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough();
                });
                //.AddValidation(options =>
                //{
                //    options.UseLocalServer();
                //    options.UseAspNetCore();
                //});

            // Add the Entity Framework Core and the default Razor Pages built-in UI
            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                //.AddDefaultUI();

            services.AddHostedService<SeedData>();

            // https://localhost:5055/.well-known/openid-configuration

            return services;
        }
    }
}
