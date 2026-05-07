using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WeatherAPI.Models
{
   public class TemperatureResult
   {
      public double TemperatureC { get; set; }
      public string MeasuredAtUtc { get; set; } = string.Empty;
   }
}
