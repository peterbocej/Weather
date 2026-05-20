using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WeatherAPI.Infrastructure.Repository;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   [Authorize(Roles = "Administrator")]
   public class SysController : ControllerBase
   {
      private readonly ITemperatureResultRepository _repository;
      private readonly ILogger<SysController> _logger;
      public SysController(ITemperatureResultRepository repository, ILogger<SysController> logger)
      {
         _repository = repository;
         _logger = logger;
      }
      [HttpGet]
      public async Task<IActionResult> Get()
      {
         return Ok(await Task.FromResult(new
         {
            Title = Assembly.GetEntryAssembly()?.GetName().Name,
            Assembly.GetEntryAssembly()?.GetName().Version,
            Clr = Environment.Version.ToString(),
            OperatingSystem = Environment.OSVersion
         }));
      }
      [HttpGet("about")]
      [AllowAnonymous]
      public async Task<IActionResult> About()
      {
         return Ok(await Task.FromResult(new
         {
            Title = Assembly.GetEntryAssembly()?.GetName().Name,
            Assembly = Assembly.GetEntryAssembly()?.GetName().Version
         }));
      }
      [HttpGet("cache")]
      public async Task<IActionResult> Cache()
      {
         try
         {
            return Ok(await _repository.GetAllAsync());
         }
         catch (Exception ex)
         {
            return BadRequest(ex.Message);
         }
      }
   }
}
