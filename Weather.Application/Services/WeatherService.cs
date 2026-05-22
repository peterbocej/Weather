using Microsoft.Extensions.Logging;
using Weather.Domain;
using Weather.Domain.Exceptions;
using Weather.Domain.Models;
using Weather.Infrastructure.ExternalApi;
using Weather.Infrastructure.Repository;

namespace Weather.Application.Services
{
   public interface IWeatherService
   {
      Task<TemperatureResult> GetTemperatureAsync(int cityId);
   }
   public class WeatherService : IWeatherService
   {
      private readonly ITemperatureResultRepository _temperatureResultRepository;
      private readonly IExternalWeatherApi _externalWeather;
      private readonly AppSettings _settings;
      private readonly ILogger<WeatherService> _logger;
      public WeatherService(
         ITemperatureResultRepository temperatureResultRepository,
         ILogger<WeatherService> logger,
         IExternalWeatherApi externalWeather,
         AppSettings settings)
      {
         _temperatureResultRepository = temperatureResultRepository;
         _logger = logger;
         _externalWeather = externalWeather;
         _settings = settings;
      }
      public async Task<TemperatureResult> GetTemperatureAsync(int cityId)
      {
         var temperatureResult = await _temperatureResultRepository.GetByIdAsync(cityId);
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
            if (await _externalWeather.FetchTemperatureFromApiAsync(temperatureResult))
            {
               if (_settings.Cache.Mode != CacheMode.None)
               {
                  await _temperatureResultRepository.UpdateAsync(temperatureResult);
                  await _temperatureResultRepository.SaveAsync(null);
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
