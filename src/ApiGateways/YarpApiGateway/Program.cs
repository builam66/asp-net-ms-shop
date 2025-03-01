using Microsoft.AspNetCore.RateLimiting;
using YarpApiGateway;

var builder = WebApplication.CreateBuilder(args);

// Services
//builder.Services.AddReverseProxy()
//    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddYarpReverseProxy(builder.Configuration);

builder.Services.AddIdentityServiceAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});

var app = builder.Build();

// HTTP request
app.UseRateLimiter();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
//.RequireAuthorization(); // Enforces auth on all proxied routes if needed

app.Run();
