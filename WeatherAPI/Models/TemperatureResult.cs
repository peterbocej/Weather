using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WeatherAPI.Models
{
   public class TemperatureResult
   {
      public int Id { get; set; }
      public string City { get; set; } = string.Empty;
      public double? TemperatureC { get; set; }
      public DateTime? MeasuredAtUtc { get; set; }
   }
}
