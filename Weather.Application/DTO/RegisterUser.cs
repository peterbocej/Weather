using Weather.Domain.Models.Security;

namespace Weather.Application.DTO
{
   public class RegisterUser : User
   {
      public string PasswordConfirmation { get; set; } = string.Empty;
   }
}
