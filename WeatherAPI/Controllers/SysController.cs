using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   [Authorize(Roles = "Admin")]
   public class SysController : ControllerBase
   {
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
   }
}
