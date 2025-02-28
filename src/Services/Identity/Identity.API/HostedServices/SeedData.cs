namespace Identity.API.HostedServices
{
    public class SeedData(IServiceProvider _serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var serviceScope = _serviceProvider.CreateScope();

            await PopulateScopesAsync(serviceScope, cancellationToken);
            await PopulateInternalAppsAsync(serviceScope, cancellationToken);
            await PopulateUsersAsync(serviceScope);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        static async ValueTask PopulateInternalAppsAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
        {
            // var context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
            // await context.Database.EnsureCreatedAsync(cancellationToken);
            var appManager = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            var appDescriptor = new OpenIddictApplicationDescriptor()
            {
                ClientId = "yarp_gateway_client",
                ClientSecret = "yarp_gateway_secret",
                DisplayName = "Yarp Gateway",
                //RedirectUris = { new Uri("https://localhost:5055/callback") },
                //RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                ClientType = ClientTypes.Confidential,
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    //Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.EndSession,

                    //Permissions.GrantTypes.ClientCredentials,
                    //Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.GrantTypes.Password,

                    Permissions.Prefixes.Scope + "yarp_gateway_scope",

                    Permissions.ResponseTypes.Code,
                },
            };

            var client = await appManager.FindByClientIdAsync(appDescriptor.ClientId, cancellationToken);
            if (client == null)
            {
                await appManager.CreateAsync(appDescriptor, cancellationToken);
            }
            else
            {
                await appManager.UpdateAsync(client, appDescriptor, cancellationToken);
            }
        }

        static async ValueTask PopulateScopesAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
        {
            var scopeManager = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            var scopeDescriptor = new OpenIddictScopeDescriptor
            {
                Name = "yarp_gateway_scope",
                Resources = { "yarp_gateway" }
            };

            var scopeInstance = await scopeManager.FindByNameAsync(scopeDescriptor.Name, cancellationToken);
            if (scopeInstance == null)
            {
                await scopeManager.CreateAsync(scopeDescriptor, cancellationToken);
            }
            else
            {
                await scopeManager.UpdateAsync(scopeInstance, scopeDescriptor, cancellationToken);
            }
        }

        static async ValueTask PopulateUsersAsync(IServiceScope serviceScope)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser("test_user@gmail.com");
            await userManager.CreateAsync(user, "Pass@word1");
        }
    }
}
