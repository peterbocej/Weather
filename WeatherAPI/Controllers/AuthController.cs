using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using WeatherAPI.Domain.DTO;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AuthController : ControllerBase
   {
      private readonly IConfiguration _config;
      private readonly ILogger<AuthController> _logger;
      public AuthController(IConfiguration config, ILogger<AuthController> logger)
      {
         _config = config;
         _logger = logger;
      }

      [HttpGet("user")]
      [Authorize]
      public IActionResult GetUser()
      {
         return Ok(new
         {
            User.Identity?.Name
         });
      }

      [HttpPost("login")]
      public IActionResult Login([FromBody] LoginRequest request)
      {
         if (!ValidateUser(request.User, request.Password))
         {
            _logger.LogWarning("Invalid login attempt for user {user}", request.User);
            return Unauthorized();
         }

         var token = GenerateToken(request.User, request.Password);
         if (token == null)
         {
            _logger.LogWarning("Invalid login attempt for user {user}", request.User);
            return Unauthorized();
         }

         _logger.LogInformation("User {user} logged in successfully", request.User);
         return Ok(token);
      }

      private bool ValidateUser(string username, string password)
      {
         if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
         { 
            _logger.LogWarning("Username or password is empty");
            return false;
         }
         var hashedPassword = HashPassword(password);
         foreach (var user in _config.GetSection("Users").GetChildren())
            if (user["Username"] == username && user["Password"] == hashedPassword)
               return true;

         return false;
      }

      private string? HashPassword(string password)
      {
         // For demonstration purposes only. In production, use a secure hashing algorithm like bcrypt or Argon2.
         return password;
      }

      private string GenerateToken(string user, string password)
      {
         var role = _config.GetSection("Users").GetChildren()
            .FirstOrDefault(u => u["Username"] == user)?["Role"];
         var claims = new[]
         {
            new Claim(ClaimTypes.Name, user),
            new Claim(ClaimTypes.Role, role ?? string.Empty)
         };
         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

         var token = new JwtSecurityToken(
             issuer: _config["Jwt:Issuer"],
             audience: _config["Jwt:Audience"],
             claims: claims,
             expires: DateTime.Now.AddHours(1),
             signingCredentials: creds
         );

         var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
         return $"Bearer {tokenString}";
      }
   }
}
