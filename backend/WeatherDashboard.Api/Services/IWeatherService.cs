using WeatherDashboard.Api.Models;

namespace WeatherDashboard.Api.Services;

public interface IWeatherService
{
    Task<WeatherSnapshot> GetByCityAsync(string city, CancellationToken cancellationToken = default);
}
