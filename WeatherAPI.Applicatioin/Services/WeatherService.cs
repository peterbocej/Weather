using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherAPI.Domain.Models;
using WeatherAPI.Infrastructure.ExternalApi;
using WeatherAPI.Infrastructure.Repository;

namespace WeatherAPI.Application.Services
{
   public interface IWeatherService
   {
      Task<TemperatureResult> GetTemperatureAsync(int cityId);
   }
   public class WeatherService : IWeatherService
   {
      private readonly ITemperatureResultRepository _temperatureResultRepository;
      private readonly IExternalWeatherApi _externalWeatherApi;
      private readonly ILogger<WeatherService> _logger;
      public WeatherService(
         ITemperatureResultRepository temperatureResultRepository, 
         ILogger<WeatherService> logger, 
         IExternalWeatherApi externalWeatherApi)
      {
         _temperatureResultRepository = temperatureResultRepository;
         _logger = logger;
         _externalWeatherApi = externalWeatherApi;
      }
      public async Task<TemperatureResult> GetTemperatureAsync(int cityId)
      {
         var temperatureResult = await _temperatureResultRepository.GetTemperatureResultAsync(cityId);
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
            if (await _externalWeatherApi.FetchTemperatureFromApiAsync(temperatureResult))
            {
               _temperatureResultRepository.UpdateAsync(temperatureResult);
               await _temperatureResultRepository.SaveChangesAsync();
               return temperatureResult;
            }
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
   }
}
