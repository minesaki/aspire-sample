public class WebAPIClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherForecastAsync()
    {
        return await httpClient.GetFromJsonAsync<WeatherForecast[]>("weatherforecast") ?? [];
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}