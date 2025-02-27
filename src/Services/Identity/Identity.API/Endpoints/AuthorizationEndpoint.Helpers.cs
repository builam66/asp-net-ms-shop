namespace Identity.API.Endpoints
{
    public partial class AuthorizationEndpoint
    {
        static IEnumerable<string> GetDestinations(Claim claim)
        {
            switch (claim.Type)
            {
                case Claims.Name or Claims.PreferredUsername:
                    yield return Destinations.AccessToken;

                    if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }

        static IResult Deny()
        {
            return Results.Forbid(authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
        }

        static async Task<IResult> Accept(
            HttpContext httpContext,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager,
            IOpenIddictScopeManager scopeManager,
            UserManager<IdentityUser> userManager)
        {
            var request = httpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var user = await userManager.GetUserAsync(httpContext.User) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            var application = await applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            var authorizationList = authorizationManager.FindAsync(
                subject: await userManager.GetUserIdAsync(user),
                client: await applicationManager.GetIdAsync(application),
                status: Statuses.Valid,
                type: AuthorizationTypes.Permanent,
                scopes: request.GetScopes());
            var authorizations = new List<object>();
            await foreach (object auth in authorizationList)
            {
                authorizations.Add(auth);
            }

            if (authorizations.Count is 0 && await applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
            {
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }!));
            }

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity
                .SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, [.. (await userManager.GetRolesAsync(user))]);

            identity.SetScopes(request.GetScopes());
            var scopeResources = scopeManager.ListResourcesAsync(identity.GetScopes());
            var resources = new List<string>();
            await foreach (string resource in scopeResources)
            {
                resources.Add(resource);
            }
            identity.SetResources(resources);

            var authorization = authorizations.LastOrDefault();
            authorization ??= await authorizationManager.CreateAsync(
                identity: identity,
                subject: await userManager.GetUserIdAsync(user),
                client: await applicationManager.GetIdAsync(application),
                type: AuthorizationTypes.Permanent,
                scopes: identity.GetScopes());

            identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
            identity.SetDestinations(GetDestinations);

            return Results.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        static async Task<IResult?> VerifyConsent(
            HttpContext httpContext,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager,
            IOpenIddictScopeManager scopeManager,
            UserManager<IdentityUser> userManager)
        {
            if (httpContext.Request.Method != "POST")
                return null;

            if (httpContext.Request.Form.Where(parameter => parameter.Key == "submit.Accept").Any())
                return await Accept(httpContext, applicationManager, authorizationManager, scopeManager, userManager);

            if (httpContext.Request.Form.Where(parameter => parameter.Key == "submit.Deny").Any())
                return Deny();

            return null;
        }
    }
}
