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

      public TemperatureController(IWeatherService weatherService)
      {
         _weatherService = weatherService;
      }

      [HttpGet("{city}")]
      public async Task<IActionResult> GetTemperature(int city)
      {
         try
         {
            var result = await _weatherService.GetTemperatureAsync(city);
            return Ok(result);
         }
         catch (Exception ex)
         {
            return BadRequest(ex.Message);
         }
      }
   }
}
