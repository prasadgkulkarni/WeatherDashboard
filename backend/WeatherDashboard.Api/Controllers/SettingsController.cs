using Microsoft.AspNetCore.Mvc;
using WeatherDashboard.Api.Infrastructure;
using WeatherDashboard.Api.Models;

namespace WeatherDashboard.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly IDefaultLocationStore _defaultLocationStore;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(IDefaultLocationStore defaultLocationStore, ILogger<SettingsController> logger)
    {
        _defaultLocationStore = defaultLocationStore;
        _logger = logger;
    }

    [HttpGet("default-location")]
    public async Task<ActionResult<DefaultLocationDto>> GetDefaultLocation(CancellationToken cancellationToken)
    {
        try
        {
            var city = await _defaultLocationStore.GetAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(city))
            {
                return NotFound();
            }

            return Ok(new DefaultLocationDto { City = city });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting default location");
            throw;
        }
    }

    [HttpPut("default-location")]
    public async Task<IActionResult> SetDefaultLocation(DefaultLocationDto body, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _defaultLocationStore.SetAsync(body.City, cancellationToken);
            _logger.LogInformation("Default location set to {City}", body.City);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error setting default location to {City}", body.City);
            throw;
        }
    }
}
