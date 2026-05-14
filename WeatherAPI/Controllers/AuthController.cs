using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AuthController : ControllerBase
   {
      private readonly IConfiguration _config;
      private readonly IJwtService _jwtService;
      private readonly ILogger<AuthController> _logger;
      public AuthController(IConfiguration config, IJwtService jwtService, ILogger<AuthController> logger)
      {
         _config = config;
         _jwtService = jwtService;
         _logger = logger;
      }

      [HttpPost("login")]
      public IActionResult Login([FromBody] LoginRequest request)
      {
         if (!ValidateUser(request.User, request.Password))
         {
            _logger.LogWarning("Invalid login attempt for user {user}", request.User);
            return Unauthorized();
         }

         var token = _jwtService.GenerateToken(request.User, request.Password);
         if (token == null)
         {
            _logger.LogWarning("Invalid login attempt for user {user}", request.User);
            return Unauthorized();
         }

         _logger.LogInformation("User {user} logged in successfully", request.User);
         return Ok(new { Token = token });
      }
      private bool ValidateUser(string username, string password)
      {
         foreach (var user in _config.GetSection("Users").GetChildren())
            if (user["Username"] == username && user["Password"] == password)
               return true;

         return false;
      }

   }
}
