using System.Collections.Concurrent;

namespace WeatherDashboard.Api.Infrastructure;

public class DefaultLocationStore : IDefaultLocationStore
{
    private readonly ConcurrentDictionary<string, string> _state = new();
    private const string Key = "default";

    public Task<string?> GetAsync(CancellationToken cancellationToken = default)
    {
        _state.TryGetValue(Key, out var city);
        return Task.FromResult(city);
    }

    public Task SetAsync(string city, CancellationToken cancellationToken = default)
    {
        _state[Key] = city;
        return Task.CompletedTask;
    }
}
