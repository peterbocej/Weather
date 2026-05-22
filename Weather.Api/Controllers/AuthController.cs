using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.Application.DTO;
using Weather.Domain.Extensions;
using WebApi8.Services;

namespace Weather.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
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
            _logger.LogError(ex, "Error registering user {Username}", registerUser.UserName);
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
            _logger.LogError(ex, "Error logging in user {Username}", login.UserName);
            return BadRequest(ex.GetFullMessage());
         }
      }
      [HttpPost("validate-token")]
      public async Task<IActionResult> ValidateToken([FromBody] string token)
      {
         try
         {
            // Implementation for token validation
            return Ok("Token is valid");
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error validating token");
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
            return Ok(new 
            {
               user.Id,
               user.UserName,
               user.UserEmail,
               user.Role
            });
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error fetching user {Username}", User.Identity?.Name);
            return BadRequest(ex.GetFullMessage());
         }
      }
      [HttpGet("users")]
      [Authorize(Roles = "Administrator")]
      public async Task<IActionResult> GetUsers()
      {
         try
         {
            var users = await _userService.GetAllUsers();
            return Ok(users.Select(u => new 
            { 
               u.Id, 
               u.UserName, 
               u.UserEmail, 
               u.Role
            }));
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error fetching users");
            return BadRequest(ex.GetFullMessage());
         }
      }
   }
}
