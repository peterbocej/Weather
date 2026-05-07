using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
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
      public async Task<IActionResult> GetTemperature(int cityId)
      {
         _logger.LogInformation("{date} - GET request for temperature of cityId {CityId}", DateTime.UtcNow, cityId);
         try
         {
            var result = await _weatherService.GetTemperatureAsync(cityId);
            return Ok(result);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error occurred while fetching temperature for cityId {CityId}", cityId);
            return BadRequest(ex.Message);
         }
      }
   }
}
