using Microsoft.Extensions.Logging;
using System.Text.Json;
using Weather.Domain;
using Weather.Domain.Models;

namespace Weather.Infrastructure.ExternalApi
{
   public interface IExternalWeatherApi
   {
      Task<bool> FetchTemperatureFromApiAsync(TemperatureResult temperatureResult);
   }
   public class ExternalWeatherApi : IExternalWeatherApi
   {
      private readonly AppSettings _appSettings;
      private readonly ILogger<ExternalWeatherApi> _logger;
      public ExternalWeatherApi(AppSettings appSettings, ILogger<ExternalWeatherApi> logger)
      {
         _appSettings = appSettings;
         _logger = logger;
      }
      public async Task<bool> FetchTemperatureFromApiAsync(TemperatureResult temperatureResult)
      {
         var cityName = temperatureResult.City;
         var requestUrl = string.Format(_appSettings.WeatherApiServer.Url, _appSettings.WeatherApiServer.ApiKey, cityName);
         // Log the request URL for debugging purposes
         _logger.LogInformation("Fetching weather data from API for city: {CityName} using URL: {RequestUrl}", cityName, requestUrl);
         using (var semaphore = new SemaphoreSlim(1, 3))
         {
            await semaphore.WaitAsync();
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
}
