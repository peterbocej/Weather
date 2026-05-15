using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   [Authorize]
   public class TemperatureController : ControllerBase
   {
      private readonly IWeatherService _weatherService;
      private readonly ILogger<TemperatureController> _logger;

      public TemperatureController(IWeatherService weatherService, ILogger<TemperatureController> logger)
      {
         _weatherService = weatherService;
         _logger = logger;
      }

      [HttpGet("{cityId}")]
      public async Task<IActionResult> GetCityTemperature(int cityId)
      {
         _logger.LogInformation("{date} - GET request for temperature of cityId {CityId}", DateTime.UtcNow, cityId);
         try
         {
            var result = await _weatherService.GetTemperatureAsync(cityId);
            if (result == null || result.TemperatureC == null || result.MeasuredAtUtc == null)
            {
               _logger.LogWarning("No temperature data found for cityId {CityId}", cityId);
               return NotFound($"No temperature data found for cityId {cityId}");
            }
            return Ok(new
            {
               Temperature = result.TemperatureC,
               MeasuredAt = result.MeasuredAtUtc.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
            });
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error occurred while fetching temperature for cityId {CityId}", cityId);
            return BadRequest(ex.Message);
         }
      }
   }
}
