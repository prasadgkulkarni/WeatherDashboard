using System.Threading.Tasks;
using WeatherDashboard.Api.Infrastructure;
using Xunit;

namespace WeatherDashboard.Tests;

public class DefaultLocationStoreTests
{
    [Fact]
    public async Task Stores_and_retrieves_default_city()
    {
        var store = new DefaultLocationStore();

        await store.SetAsync("Berlin");
        var city = await store.GetAsync();

        Assert.Equal("Berlin", city);
    }
}
