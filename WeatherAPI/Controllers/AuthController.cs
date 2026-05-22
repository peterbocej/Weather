using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Application.DTO;
using WeatherAPI.Domain.Extensions;
using WebApi8.Services;

namespace WeatherAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   [Authorize]
   public class AuthController : ControllerBase
   {
      private readonly IUserService _userService;
      private readonly ILogger<AuthController> _logger;
      public AuthController(IUserService userService, ILogger<AuthController> logger)
      {
         _userService = userService;
         _logger = logger;
      }

      [HttpPost("register")]
      public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
      {
         try
         {
            await _userService.RegisterUser(registerUser);
            return Ok($"User {registerUser.UserName} registered successfully");
         }
         catch (Exception ex)
         {
            return BadRequest(ex.GetFullMessage());
         }
      }
      [HttpPost("login")]
      public async Task<IActionResult> Login([FromBody] Login login)
      {
         try
         {
            var token = await _userService.Login(login);
            return Ok(token);
         }
         catch (Exception ex)
         {
            return BadRequest(ex.GetFullMessage());
         }
      }
      [HttpGet("validate")]
      public async Task<IActionResult> Validate([FromQuery] string token)
      {
         try
         {
            // Implementation for token validation
            return Ok("Token is valid");
         }
         catch (Exception ex)
         {
            return BadRequest(ex.GetFullMessage());
         }
      }
      [HttpGet("validate-user")]
      public async Task<IActionResult> ValidateUser([FromQuery] string username)
      {
         try
         {
            // Implementation for user validation
            return Ok("User is valid");
         }
         catch (Exception ex)
         {
            return BadRequest(ex.GetFullMessage());
         }
      }
      [HttpGet("user")]
      [Authorize]
      public async Task<IActionResult> GetUser()
      {
         try
         {
            var user = await _userService.GetUserByUsername(User.Identity?.Name!);
            return Ok(user);
         }
         catch (Exception ex)
         {
            return BadRequest(ex.GetFullMessage());
         }
      }
   }
}
