using System.Text.Json;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
   public interface IWeatherService
   {
      Task<TemperatureResult> GetTemperatureAsync(int cityId);
   }
   public class WeatherService : IWeatherService
   {
      private readonly IConfiguration _configuration;
      private readonly IDictionary<int, string> _cities;
      private readonly string? _url;
      private readonly string? _apiKey;
      public WeatherService(IConfiguration configuration)
      {
         _configuration = configuration;
         _cities = new Dictionary<int, string>
         {
            { 1, "bratislava" },
            { 2, "praha" },
            { 3, "budapest" },
            { 4, "vienna" }
         };
         _url = _configuration.GetValue<string>("Server:Url");
         _apiKey = _configuration.GetValue<string>("Server:ApiKey");
      }
      public async Task<TemperatureResult> GetTemperatureAsync(int cityId)
      {
         if (!_cities.ContainsKey(cityId))
            throw new ArgumentOutOfRangeException(nameof(cityId), "Invalid city ID (1 - 4).");

         var cityName = _cities[cityId];
         var requestUrl = string.Format(_url!, _apiKey, cityName);
         var semafore = new SemaphoreSlim(0, 3);
         using (var client = new HttpClient())
         {
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            var temperatureC = 0.0;
            var lastUpdated = DateTime.UtcNow;
            if (dict != null && dict.ContainsKey("current"))
            {
               var current = dict["current"] as JsonElement?;
               if (current == null || !current.HasValue)
                  throw new Exception("Current weather data not found in API response.");

               if (current.Value.TryGetProperty("temp_c", out var temp_c))
                  temperatureC = temp_c.GetDouble();
               else
                  throw new Exception("Temperature data not found in API response.");

               if (!(current.Value.TryGetProperty("last_updated", out var last_updated)
                  && DateTime.TryParse(last_updated.GetString(), out lastUpdated)))
                  throw new Exception("Last updated data not found in API response.");
               
               return new TemperatureResult
               {
                  TemperatureC = temperatureC,
                  MeasuredAtUtc = lastUpdated.ToString("yyyy-MM-ddTHH:mm:ssZ")
               };
            }
            else
            {
               throw new Exception("Invalid response from weather API.");
            }
         }
      }
   }
}
