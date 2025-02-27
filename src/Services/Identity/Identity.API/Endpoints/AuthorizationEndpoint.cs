namespace Identity.API.Endpoints
{
    public partial class AuthorizationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/connect/token", Exchange);
            app.MapGet("/connect/authorize", Authorize);
            app.MapPost("/connect/authorize", Authorize);
            app.MapPost("/connect/user/register", RegisterUser);
        }
    }
}
