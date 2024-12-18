var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

// Services
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

var app = builder.Build();

// Http request pipeline
app.MapCarter();

app.Run();
