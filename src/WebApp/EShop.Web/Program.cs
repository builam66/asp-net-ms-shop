using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

var apiGatewayUri = new Uri(builder.Configuration["ApiSettings:APIGateway"]!);

// Add services to the container.
builder.Services.AddRazorPages();

// Handle message after request and before response
builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = apiGatewayUri;
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = apiGatewayUri;
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = apiGatewayUri;
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
