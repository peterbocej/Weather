using WeatherAPI.Domain.Models.Security;

namespace WeatherAPI.Application.DTO
{
   public class RegisterUser : User
   {
      public string PasswordConfirmation { get; set; } = string.Empty;
   }
}
