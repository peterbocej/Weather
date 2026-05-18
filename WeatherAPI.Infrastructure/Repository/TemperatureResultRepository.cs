using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WeatherAPI.Domain;
using WeatherAPI.Domain.Models;
using WeatherAPI.Infrastructure.Data;

namespace WeatherAPI.Infrastructure.Repository
{
   public interface ITemperatureResultRepository : IDisposable
   {
      Task<IEnumerable<TemperatureResult>> GetAllAsync();
      Task<TemperatureResult?> GetTemperatureResultAsync(int cityId);
      EntityEntry<TemperatureResult> UpdateAsync(TemperatureResult temperatureResult);
      Task<int> SaveChangesAsync();
   }
   public class TemperatureResultRepository : ITemperatureResultRepository
   {
      private readonly CacheDbContext _context;
      private readonly Settings _settings;
      public TemperatureResultRepository(CacheDbContext context, Settings settings)
      {
         _context = context;
         _settings = settings;
      }

      public async Task<IEnumerable<TemperatureResult>> GetAllAsync()
      {
         return await _context.TemperatureResults.ToArrayAsync();
      }

      public async Task<TemperatureResult?> GetTemperatureResultAsync(int cityId)
      {
         return await _context.TemperatureResults.FindAsync(cityId);
      }

      public Task<int> SaveChangesAsync()
      {
         return _context.SaveChangesAsync();
      }

      public EntityEntry<TemperatureResult> UpdateAsync(TemperatureResult temperatureResult)
      {
         return _context.TemperatureResults.Update(temperatureResult);
      }

      public void Dispose()
      {
         _context?.Dispose();
      }
   }
}