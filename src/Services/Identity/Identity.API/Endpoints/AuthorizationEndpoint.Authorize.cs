namespace Identity.API.Endpoints
{
    public partial class AuthorizationEndpoint
    {
        public static async Task<IResult> Authorize(
            HttpContext httpContext,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager,
            IOpenIddictScopeManager scopeManager,
            UserManager<IdentityUser> userManager)
        {
            var consentVerified = await VerifyConsent(httpContext, applicationManager, authorizationManager, scopeManager, userManager);
            if (consentVerified is not null)
            {
                return consentVerified;
            }

            var request = httpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var result = await httpContext.AuthenticateAsync();
            if (result == null || !result.Succeeded || request.HasPromptValue(PromptValues.Login))
            {
                if (request.HasPromptValue(PromptValues.None))
                {
                    return Results.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                        }!)
                    );
                }

                var prompt = string.Join(" ", request.GetPromptValues().Remove(PromptValues.Login));

                var parameters = httpContext.Request.HasFormContentType ?
                    httpContext.Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList() :
                    httpContext.Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

                parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

                return Results.Challenge(new AuthenticationProperties
                {
                    RedirectUri = httpContext.Request.PathBase + httpContext.Request.Path + QueryString.Create(parameters)
                });
            }

            var user = await userManager.GetUserAsync(result.Principal) ??
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
            await foreach (object authorization in authorizationList)
            {
                authorizations.Add(authorization);
            }

            switch (await applicationManager.GetConsentTypeAsync(application))
            {
                case ConsentTypes.External when authorizations.Count is 0:
                    return Results.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "The logged in user is not allowed to access this client application."
                        }!));

                case ConsentTypes.Implicit:
                case ConsentTypes.External when authorizations.Count is not 0:
                case ConsentTypes.Explicit when authorizations.Count is not 0 && !request.HasPromptValue(PromptValues.Consent):

                    ClaimsIdentity identity = new(
                        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                        nameType: Claims.Name,
                        roleType: Claims.Role);

                    identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
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

                case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
                case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                    return Results.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Interactive user consent is required."
                        }!)
                    );

                default:
                    string jsonData = $"{{  \"applicationName\": \"{await applicationManager.GetLocalizedDisplayNameAsync(application)}\", \"scope\": \"{request.Scope}\"  }}";
                    httpContext.Session.SetString("ConsentData", jsonData);
                    IEnumerable<KeyValuePair<string, StringValues>> parameters = httpContext.Request.HasFormContentType ?
                        httpContext.Request.Form : httpContext.Request.Query;
                    return Results.Redirect($"/Consent{QueryString.Create(parameters)}");
            }
        }
    }
}
