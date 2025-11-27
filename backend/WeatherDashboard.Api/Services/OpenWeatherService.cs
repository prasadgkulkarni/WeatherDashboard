using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using WeatherDashboard.Api.Models;

namespace WeatherDashboard.Api.Services;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private const string CachePrefix = "weather-city-";

    public OpenWeatherService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<WeatherSnapshot> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CachePrefix}{city.ToLowerInvariant()}";
        if (_cache.TryGetValue(cacheKey, out WeatherSnapshot? cached) && cached is not null)
        {
            return cached;
        }

        var apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("OpenWeather API key is missing. Set OpenWeather__ApiKey in configuration.");
        }

        var url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"City '{city}' was not found.");
        }

        await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var json = await JsonSerializer.DeserializeAsync<JsonElement>(content, SerializerOptions, cancellationToken);

        var snapshot = MapSnapshot(city, json);

        _cache.Set(cacheKey, snapshot, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return snapshot;
    }

    private static WeatherSnapshot MapSnapshot(string city, JsonElement json)
    {
        var main = json.GetProperty("main");
        var wind = json.GetProperty("wind");
        var weather = json.GetProperty("weather")[0];

        return new WeatherSnapshot(
            city,
            TemperatureC: main.GetProperty("temp").GetDouble(),
            Humidity: main.GetProperty("humidity").GetDouble(),
            WindSpeed: wind.GetProperty("speed").GetDouble(),
            Description: weather.GetProperty("description").GetString() ?? "Unknown",
            Icon: weather.GetProperty("icon").GetString() ?? "01d");
    }
}
