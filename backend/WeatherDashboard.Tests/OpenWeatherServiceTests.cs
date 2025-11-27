using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using WeatherDashboard.Api.Services;
using Xunit;

namespace WeatherDashboard.Tests;

public class OpenWeatherServiceTests
{
    [Fact]
    public async Task Caches_responses_by_city()
    {
        var handler = new StubHttpMessageHandler();
        var client = new HttpClient(handler);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenWeather:ApiKey"] = "test-key"
            })
            .Build();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OpenWeatherService(client, configuration, cache);

        var first = await service.GetByCityAsync("London");
        var second = await service.GetByCityAsync("London");

        Assert.Equal(first.TemperatureC, second.TemperatureC);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task Throws_when_api_key_missing()
    {
        var handler = new StubHttpMessageHandler();
        var client = new HttpClient(handler);
        var configuration = new ConfigurationBuilder().Build();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OpenWeatherService(client, configuration, cache);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetByCityAsync("Paris"));
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            var json = """
            {
              "weather": [{ "description": "clear sky", "icon": "01d" }],
              "main": { "temp": 15.2, "humidity": 61 },
              "wind": { "speed": 3.4 }
            }
            """;
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            return Task.FromResult(message);
        }
    }
}
