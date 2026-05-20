using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WeatherAPI.Application.Services;
using WeatherAPI.Domain.Exceptions;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   [Authorize(Roles = "User,Administrator")]
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
            if (result == null || result.TemperatureC == null || result.MeasuredAtUTC == null)
            {
               _logger.LogWarning("No temperature data found for cityId {CityId}", cityId);
               return NotFound($"No temperature data found for cityId {cityId}");
            }
            return Ok(new
            {
               Temperature = result.TemperatureC.Value,
               MeasuredAtUtc = result.MeasuredAtUTC.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
            });
         }
         catch (NotFoundException ex)
         {
            _logger.LogWarning(ex, "City not found for cityId {CityId}", cityId);
            return NotFound(ex.FullMessage);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error occurred while fetching temperature for cityId {CityId}", cityId);
            return BadRequest(ex.Message);
         }
      }
   }
}
