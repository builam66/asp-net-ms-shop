var builder = WebApplication.CreateBuilder(args);

// Services 

var app = builder.Build();

// Http request pipeline

app.Run();
