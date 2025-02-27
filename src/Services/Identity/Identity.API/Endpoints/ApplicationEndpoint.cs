using System.Text.Json;

namespace Identity.API.Endpoints
{
    public class ApplicationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/callback", Callback);
        }

        public static async Task<IResult> Callback(HttpContext httpContext, IHttpClientFactory httpClientFactory)
        {
            IEnumerable<KeyValuePair<string, StringValues>> parameters = httpContext.Request.HasFormContentType ?
                httpContext.Request.Form : httpContext.Request.Query;

            var formData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code_verifier", "AVA~cbYg_UDgPYrJNJX.kMotv0x.z8nY~C23XzWq4DxEUu0cw9rWk6SwlgHgihmBoPN4.WKV0H1ui6TTL3vCWC0jyv7fYlAef3Z-y-7rgC6~0m9bb06x8FEO24LJArH4" },
                { "client_id", "test_client" },
                { "client_secret", "test_secret" },
                { "redirect_uri", "https://localhost:5055/callback" }
            };
            var codeParameter = parameters.First(p => p.Key == "code");
            formData.Add(codeParameter.Key, codeParameter.Value);

            var httpClient = httpClientFactory.CreateClient("TokenApiClient");
            var content = new FormUrlEncodedContent(formData);
            var response = await httpClient.PostAsync("connect/token", content);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<dynamic>(responseContent);

            return Results.Json(jsonObject);
        }
    }
}
