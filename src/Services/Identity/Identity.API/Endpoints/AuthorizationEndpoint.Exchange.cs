namespace Identity.API.Endpoints
{
    public partial class AuthorizationEndpoint
    {
        public static async Task<IResult> Exchange(
            HttpContext httpContext,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            var request = httpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsClientCredentialsGrantType())
            {
                return await ClientCredentialsExchange(request, applicationManager, scopeManager);
            }

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                var authenticateResult = await httpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                return await AuthorizationCodeAndRefreshTokenExchange(authenticateResult, userManager, signInManager);
            }

            if (request.IsPasswordGrantType())
            {
                return await PasswordExchange(request, scopeManager, userManager, signInManager);
            }

            return Results.BadRequest(new
            {
                error = Errors.UnsupportedGrantType,
                error_description = "The specified grant type is not supported."
            });
        }

        private static async Task<IResult> ClientCredentialsExchange(
            OpenIddictRequest request,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager)
        {
            var application = await applicationManager.FindByClientIdAsync(request.ClientId!)
                    ?? throw new InvalidOperationException("The application details cannot be found in the database.");

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));

            identity.SetScopes(request.GetScopes());

            var scopeResources = scopeManager.ListResourcesAsync(identity.GetScopes());
            var resources = new List<string>();
            await foreach (string resource in scopeResources)
            {
                resources.Add(resource);
            }
            identity.SetResources(resources);
            identity.SetDestinations(GetDestinations);

            var principal = new ClaimsPrincipal(identity);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private static async Task<IResult> AuthorizationCodeAndRefreshTokenExchange(
            AuthenticateResult authenticateResult,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            var user = await userManager.FindByIdAsync(authenticateResult.Principal?.GetClaim(Claims.Subject) ?? string.Empty);
            if (user is null)
            {
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                    }!)
                );
            }

            if (!await signInManager.CanSignInAsync(user))
            {
                return Results.Forbid(
                    authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                    }!)
                );
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

            identity.SetDestinations(GetDestinations);

            var principal = new ClaimsPrincipal(identity);

            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private static async Task<IResult> PasswordExchange(
            OpenIddictRequest request,
            IOpenIddictScopeManager scopeManager,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            var user = await userManager.FindByNameAsync(request.Username ?? string.Empty);

            if (user == null)
            {
                return Results.BadRequest(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "User does not exist"
                });
            }

            // Check that the user can sign in and is not locked out.
            // If two-factor authentication is supported, it would also be appropriate to check that 2FA is enabled for the user
            if (!await signInManager.CanSignInAsync(user) || (userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user)))
            {
                // Return bad request is the user can't sign in
                return Results.BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.InvalidGrant,
                    ErrorDescription = "The specified user cannot sign in."
                });
            }

            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, Claims.Name, Claims.Role);

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = await signInManager.PasswordSignInAsync(user.UserName!, request.Password!, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                {
                    return Results.BadRequest(new OpenIddictResponse
                    {
                        Error = Errors.InvalidGrant,
                        ErrorDescription = "User not allowed to login. Please confirm your email"
                    });
                }

                if (result.RequiresTwoFactor)
                {
                    return Results.BadRequest(new OpenIddictResponse
                    {
                        Error = Errors.InvalidGrant,
                        ErrorDescription = "User requires 2F authentication"
                    });
                }

                if (result.IsLockedOut)
                {
                    return Results.BadRequest(new OpenIddictResponse
                    {
                        Error = Errors.InvalidGrant,
                        ErrorDescription = "User is locked out"
                    });
                }
                else
                {
                    return Results.BadRequest(new OpenIddictResponse
                    {
                        Error = Errors.InvalidGrant,
                        ErrorDescription = "Username or password is incorrect"
                    });
                }
            }

            // The user is now validated, so reset lockout counts, if necessary
            if (userManager.SupportsUserLockout)
            {
                await userManager.ResetAccessFailedCountAsync(user);
            }

            //// Getting scopes from user parameters (TokenViewModel) and adding in Identity 
            identity.SetScopes(request.GetScopes());

            // Getting scopes from user parameters (TokenViewModel)
            // Checking in OpenIddictScopes tables for matching resources
            // Adding in Identity
            var asyncResources = scopeManager.ListResourcesAsync(identity.GetScopes());
            var resources = new List<string>();
            await foreach (var resource in asyncResources)
            {
                resources.Add(resource);
            }
            identity.SetResources(resources);

            // Add Custom claims
            // sub claims is mendatory
            identity.AddClaim(new Claim(Claims.Subject, user.Id));
            identity.AddClaim(new Claim(Claims.Audience, "Resourse"));

            // Setting destinations of claims i.e. identity token or access token
            identity.SetDestinations(GetDestinations);

            return Results.SignIn(new ClaimsPrincipal(identity), new(), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
