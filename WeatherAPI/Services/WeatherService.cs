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
      private readonly IEnumerable<TemperatureResult> _cities;
      private readonly string? _url;
      private readonly string? _apiKey;
      private readonly ILogger<WeatherService> _logger;
      public WeatherService(IConfiguration configuration, ILogger<WeatherService> logger)
      {
         _configuration = configuration;
         _logger = logger;
         _cities = new List<TemperatureResult>
         {
            new TemperatureResult { Id = 1, City = "bratislava" },
            new TemperatureResult { Id = 2, City = "praha" },
            new TemperatureResult { Id = 3, City = "budapest" },
            new TemperatureResult { Id = 4, City = "vienna" }
         };
         _url = _configuration.GetValue<string>("Server:Url");
         _apiKey = _configuration.GetValue<string>("Server:ApiKey");
      }
      public async Task<TemperatureResult> GetTemperatureAsync(int cityId)
      {
         var temperatureResult = _cities.FirstOrDefault(t => t.Id == cityId);
         if (temperatureResult == null)
         {
            _logger.LogError("Invalid city ID: {CityId}", cityId);
            throw new ArgumentOutOfRangeException(nameof(cityId), "Invalid city ID (1 - 4).");
         }
         if (CheckTemperatureCache(temperatureResult))
         {
            return temperatureResult;
         }
         else
         {
            if (await FetchTemperatureFromApiAsync(temperatureResult))
               return temperatureResult;
            else
            {
               _logger.LogError("Failed to fetch temperature from API for city ID: {CityId}", cityId);
               throw new Exception("Failed to fetch temperature from API.");
            }
         }
      }

      private bool CheckTemperatureCache(TemperatureResult temperatureResult)
      {
         // check cache here
         var today9AM = DateTime.UtcNow.Date.AddHours(9);
         var today4PM = DateTime.UtcNow.Date.AddHours(16);
         var currentTime = DateTime.UtcNow;

         if (temperatureResult.MeasuredAtUtc == null || temperatureResult.TemperatureC == null)
            return false;
         var measuredAt = temperatureResult.MeasuredAtUtc.Value;

         if (measuredAt >= today9AM && measuredAt <= today4PM)
            return true;
         else if (currentTime >= today4PM)
            return true;
         
         return false;
      }
      private async Task<bool> FetchTemperatureFromApiAsync(TemperatureResult temperatureResult)
      {
         var cityName = temperatureResult.City;
         var requestUrl = string.Format(_url!, _apiKey, cityName);
         using (var client = new HttpClient())
         {
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (dict != null && dict.ContainsKey("current"))
            {
               var current = dict["current"] as JsonElement?;
               if (current == null || !current.HasValue)
               {
                  _logger.LogError("Current weather data not found in API response for city: {CityName}", cityName);
                  throw new Exception("Current weather data not found in API response.");
               }

               if (current.Value.TryGetProperty("temp_c", out var temp_c))
                  temperatureResult.TemperatureC = temp_c.GetDouble();
               else
               {
                  _logger.LogError("Temperature data not found in API response for city: {CityName}", cityName);
                  throw new Exception("Temperature data not found in API response.");
               }

               if (!(current.Value.TryGetProperty("last_updated", out var last_updated)
                  && DateTime.TryParse(last_updated.GetString(), out var lastUpdated)))
               {
                  _logger.LogError("Last updated data not found in API response for city: {CityName}", cityName);
                  throw new Exception("Last updated data not found in API response.");
               }
               temperatureResult.MeasuredAtUtc = lastUpdated;
               return true;
            }
            else
            {
               throw new Exception("Invalid response from weather API.");
            }
         }
      }
   }
}
