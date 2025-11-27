namespace WeatherDashboard.Api.Models;

public record WeatherSnapshot(
    string City,
    double TemperatureC,
    double Humidity,
    double WindSpeed,
    string Description,
    string Icon);
