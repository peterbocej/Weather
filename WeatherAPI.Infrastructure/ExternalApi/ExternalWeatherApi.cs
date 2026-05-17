using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherAPI.Domain.Models;

namespace WeatherAPI.Infrastructure.ExternalApi
{
   public interface IExternalWeatherApi
   {
      Task<bool> FetchTemperatureFromApiAsync(TemperatureResult temperatureResult);
   }
   public class ExternalWeatherApi : IExternalWeatherApi
   {
      private readonly IConfiguration _configuration;
      private readonly ILogger<ExternalWeatherApi> _logger;
      private readonly string? _url;
      private readonly string? _apiKey;
      public ExternalWeatherApi(IConfiguration configuration, ILogger<ExternalWeatherApi> logger)
      {
         _configuration = configuration;
         _logger = logger;
         _url = _configuration["Server:Url"];
         _apiKey = _configuration["Server:ApiKey"];
      }
      public async Task<bool> FetchTemperatureFromApiAsync(TemperatureResult temperatureResult)
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
