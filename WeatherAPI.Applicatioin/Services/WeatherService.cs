using Microsoft.Extensions.Logging;
using WeatherAPI.Domain;
using WeatherAPI.Domain.Exceptions;
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
      private readonly Settings _settings;
      private readonly ILogger<WeatherService> _logger;
      public WeatherService(
         ITemperatureResultRepository temperatureResultRepository,
         ILogger<WeatherService> logger,
         IExternalWeatherApi externalWeatherApi,
         Settings settings)
      {
         _temperatureResultRepository = temperatureResultRepository;
         _logger = logger;
         _externalWeatherApi = externalWeatherApi;
         _settings = settings;
      }
      public async Task<TemperatureResult> GetTemperatureAsync(int cityId)
      {
         var temperatureResult = await _temperatureResultRepository.GetTemperatureResultAsync(cityId);
         if (temperatureResult == null)
         {
            _logger.LogError("Invalid city ID: {CityId}", cityId);
            throw new NotFoundException($"City not found for ID: {cityId}");
         }
         if (CheckTemperatureCache(temperatureResult))
         {
            return temperatureResult;
         }
         else
         {
            if (await _externalWeatherApi.FetchTemperatureFromApiAsync(temperatureResult))
            {
               if (_settings.Cache.Mode != CacheMode.None)
               {
                  _temperatureResultRepository.UpdateAsync(temperatureResult);
                  await _temperatureResultRepository.SaveChangesAsync();
               }
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
         if (temperatureResult.MeasuredAtUTC == null || temperatureResult.TemperatureC == null)
            return false;
         var measuredAt = temperatureResult.MeasuredAtUTC.Value;

         if (currentTime >= today9AM && currentTime <= today4PM && measuredAt >= today9AM)
            return true;
         else if (currentTime >= today4PM && currentTime < today9AM.AddDays(1) && measuredAt >= today4PM)
            return true;

         return false;
      }
   }
}
