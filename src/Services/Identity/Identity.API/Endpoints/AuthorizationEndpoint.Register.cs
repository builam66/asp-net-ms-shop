namespace Identity.API.Endpoints
{
    public partial class AuthorizationEndpoint
    {
        public static async Task<IResult> RegisterUser(
            RegisterRequest request,
            UserManager<IdentityUser> userManager)
        {
            if (!request.TryValidate(out var validationErrors))
            {
                return Results.BadRequest(validationErrors);
            }

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                return Results.Conflict();
            }

            user = new IdentityUser { UserName = request.Username, Email = request.Email };
            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return Results.Ok();
            }

            return Results.BadRequest(result.Errors.Select(e => e.Description));
        }
    }
}
