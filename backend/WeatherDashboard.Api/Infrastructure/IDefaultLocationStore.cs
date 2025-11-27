namespace WeatherDashboard.Api.Infrastructure;

public interface IDefaultLocationStore
{
    Task<string?> GetAsync(CancellationToken cancellationToken = default);
    Task SetAsync(string city, CancellationToken cancellationToken = default);
}
