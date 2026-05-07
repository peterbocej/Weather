using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
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
   }
}
