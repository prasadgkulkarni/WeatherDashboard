using System.ComponentModel.DataAnnotations;

namespace WeatherDashboard.Api.Models;

public class DefaultLocationDto
{
    [Required]
    public string City { get; set; } = string.Empty;
}
