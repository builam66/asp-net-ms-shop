using System.Text.Json;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace YarpApiGateway.Helpers
{
    public class ModifyBodyTransformProvider : ITransformProvider
    {
        public void Apply(TransformBuilderContext context)
        {
            // Apply the custom transformation only to the token endpoint
            if (context.Route.RouteId == "login-route") // Match your YARP route ID
            {
                context.AddRequestTransform(async transformContext =>
                {
                    var request = transformContext.HttpContext.Request;

                    // Only modify requests to the token endpoint
                    if (request.Path.StartsWithSegments("/identity-service/login"))
                    {
                        // Enable buffering to read the body multiple times
                        request.EnableBuffering();

                        using var reader = new StreamReader(request.Body);
                        var body = await reader.ReadToEndAsync();
                        request.Body.Position = 0; // Reset stream position

                        try
                        {
                            // Example: Convert JSON to form data
                            var json = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body);
                            if (json == null)
                            {
                                transformContext.HttpContext.Response.StatusCode = 400;
                                return;
                            }

                            var formData = new Dictionary<string, string>();

                            foreach (var item in json)
                            {
                                formData.Add(item.Key, item.Value.ToString());
                            }
                            formData.Add("grant_type", "password");
                            formData.Add("client_id", "yarp_gateway_client");
                            formData.Add("client_secret", "yarp_gateway_secret");

                            // Replace the request content with form data
                            transformContext.ProxyRequest.Content = new FormUrlEncodedContent(formData);
                            transformContext.ProxyRequest.Content.Headers.ContentType =
                                new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        }
                        catch (JsonException)
                        {
                            transformContext.HttpContext.Response.StatusCode = 400;
                            return;
                        }
                    }
                });
            }
        }

        public void ValidateCluster(TransformClusterValidationContext context)
        {
            //throw new NotImplementedException();
        }

        public void ValidateRoute(TransformRouteValidationContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
