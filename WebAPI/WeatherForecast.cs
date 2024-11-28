using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public static class WeatherForecast
{
    public static void RegisterWeatherAPIs(this WebApplication app)
    {
        var weatherAPIs = app.MapGroup("/weatherforecast");

        weatherAPIs.MapGet("/", GetWeatherForecast);

        weatherAPIs.MapGet("/db", GetWeatherForecastFromDB);
    }

    static WeatherForecastData[]? GetWeatherForecast()
    {
        string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecastData
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    }

    static async Task<List<WeatherForecastTable>> GetWeatherForecastFromDB(WeatherForcastDbContext db)
    {
        return await db.WeatherForecasts.ToListAsync();
    }
}


public record WeatherForecastData(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record WeatherForecastTable([property: Key] Guid Id, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class WeatherForcastDbContext : DbContext
{
    public WeatherForcastDbContext(DbContextOptions<WeatherForcastDbContext> options) : base(options) { }
    public DbSet<WeatherForecastTable> WeatherForecasts { get; set; }
}