using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAPI.Domain.Models
{
   public class TemperatureResult : Entity
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public override int Id { get => base.Id; set => base.Id = value; }
      public string City { get; set; } = string.Empty;
      public double? TemperatureC { get; set; }
      public DateTime? MeasuredAtUTC { get; set; }
   }
}
