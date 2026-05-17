using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAPI.Domain.Models
{
   public class TemperatureResult
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public int TemperatureResultId { get; set; }
      public string City { get; set; } = string.Empty;
      public double? TemperatureC { get; set; }
      public DateTime? MeasuredAtUTC { get; set; }
   }
}
