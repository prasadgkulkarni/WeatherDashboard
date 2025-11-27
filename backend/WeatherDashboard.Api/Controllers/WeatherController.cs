using Microsoft.AspNetCore.Mvc;
using WeatherDashboard.Api.Models;
using WeatherDashboard.Api.Services;

namespace WeatherDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<WeatherSnapshot>> Get([FromQuery] string? city, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return BadRequest(new { message = "City is required." });
        }

        try
        {
            var snapshot = await _weatherService.GetByCityAsync(city, cancellationToken);
            return Ok(snapshot);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error fetching weather for {City}", city);
            throw; 
        }
    }
}
