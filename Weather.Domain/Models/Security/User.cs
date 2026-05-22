using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Weather.Domain.Models.Security
{
   public class User : Entity
   {
      [Key]
      [Column("UserId")]
      public override int Id { get => base.Id; set => base.Id = value; }
      public string UserName { get; set; } = string.Empty;
      [EmailAddress]
      public string UserEmail { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
      public Role Role { get; set; } = Role.None;
      [NotMapped]
      public string RoleName { get => Role.ToString(); }
   }
}
