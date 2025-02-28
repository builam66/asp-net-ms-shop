using Identity.API;

var builder = WebApplication.CreateBuilder(args);

// Services
// Transfer of consent information between the authorization endpoint and the consent page
// will be done through sessions -> add and enable session state
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(5);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

builder.Services.AddOpenIddictService(builder.Configuration);

builder.Services.AddHttpClient("TokenApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5055/");
});

builder.Services.AddCarter();

//builder.Services.AddRazorPages();

var app = builder.Build();

// HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();

//app.UseMiddleware<BodyToFormMiddleware>();

//app.UseSession();

app.UseAuthentication();
//app.UseAuthorization();

app.MapCarter();

//app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();
}

app.Run();
