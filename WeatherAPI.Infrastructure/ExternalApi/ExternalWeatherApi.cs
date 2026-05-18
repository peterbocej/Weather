using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherAPI.Domain;
using WeatherAPI.Domain.Models;

namespace WeatherAPI.Infrastructure.ExternalApi
{
   public interface IExternalWeatherApi
   {
      Task<bool> FetchTemperatureFromApiAsync(TemperatureResult temperatureResult);
   }
   public class ExternalWeatherApi : IExternalWeatherApi
   {
      private readonly Settings _settings;
      private readonly ILogger<ExternalWeatherApi> _logger;
      public ExternalWeatherApi(Settings settings, ILogger<ExternalWeatherApi> logger)
      {
         _settings = settings;
         _logger = logger;
      }
      public async Task<bool> FetchTemperatureFromApiAsync(TemperatureResult temperatureResult)
      {
         var cityName = temperatureResult.City;
         var requestUrl = string.Format(_settings.WeatherApiServer.Url, _settings.WeatherApiServer.ApiKey, cityName);
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
               temperatureResult.MeasuredAtUTC = lastUpdated.ToUniversalTime();
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
