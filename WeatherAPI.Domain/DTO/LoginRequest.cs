namespace WeatherAPI.Domain.DTO
{
   public class LoginRequest
   {
      public string User { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
   }
}
