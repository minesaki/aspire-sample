var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();   // .NET Aspire

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add database
builder.AddNpgsqlDbContext<WeatherForcastDbContext>("weatherdb");

var app = builder.Build();

app.MapDefaultEndpoints();   // .NET Aspire


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();       // Disable https

// Register APIs
app.RegisterWeatherAPIs();

app.Run();
