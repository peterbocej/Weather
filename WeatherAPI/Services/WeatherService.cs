using WeatherAPI.Models;

namespace WeatherAPI.Services
{
   public interface IWeatherService
   {
      Task<TemperatureResult> GetTemperatureAsync(int cityId);
   }
   public class WeatherService : IWeatherService
   {

      public async Task<TemperatureResult> GetTemperatureAsync(int city)
      {
         if (city < 1 || city > 4)
         {
            throw new ArgumentOutOfRangeException(nameof(city), "City ID must be between 1 and 4.");
         }
         // Simulate fetching temperature data for the specified city
         var random = new Random();
         double temperature = random.Next(-1000, 4000) / 100.0; // Random temperature between -10 and 40 degrees Celsius
         return new TemperatureResult
         {
            TemperatureC = temperature,
            MeasuredAtUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
         };
      }
   }
}
