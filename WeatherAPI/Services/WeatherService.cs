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
      }
      public async Task<TemperatureResult> GetTemperatureAsync(int cityId)
      {
         if (!_cities.ContainsKey(cityId))
         {
            throw new ArgumentOutOfRangeException(nameof(cityId), "Invalid city ID.");
         }
         var url = _configuration.GetValue<string>("Server:Url");
         var apiKey = _configuration.GetValue<string>("Server:ApiKey");
         var cityName = _cities[cityId];
         var requestUrl = string.Format(url!, apiKey, cityName);
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

               if (current.HasValue && current.Value.TryGetProperty("temp_c", out var tempC))
                  temperatureC = tempC.GetDouble();
               else
                  throw new Exception("Temperature data not found in API response.");

               if (current.HasValue 
                  && current.Value.TryGetProperty("last_updated", out var val) 
                  && val.ValueKind == JsonValueKind.String 
                  && DateTime.TryParse(val.GetString(), out var parsedDate))
                  lastUpdated = parsedDate;
               else
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
