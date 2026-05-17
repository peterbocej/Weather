using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using WeatherAPI.Domain.Models;
using WeatherAPI.Infrastructure.Data;

namespace WeatherAPI.Infrastructure.Repository
{
   public interface ITemperatureResultRepository
   {
      Task<IEnumerable<TemperatureResult>> GetAllAsync();
      Task<TemperatureResult?> GetTemperatureResultAsync(int cityId);
      EntityEntry<TemperatureResult> UpdateAsync(TemperatureResult temperatureResult);
      Task<int> SaveChangesAsync();
   }
   public class TemperatureResultRepository : ITemperatureResultRepository
   {
      private readonly CacheDbContext _context;
      public TemperatureResultRepository(CacheDbContext context)
      {
         _context = context;
         if (!_context.TemperatureResults.Any())
         {
            _context.TemperatureResults.AddRange(
               new TemperatureResult { TemperatureResultId = 1, City = "Bratislava" },
               new TemperatureResult { TemperatureResultId = 2, City = "Praha" },
               new TemperatureResult { TemperatureResultId = 3, City = "Budapest" },
               new TemperatureResult { TemperatureResultId = 4, City = "Vieden" }
            );
            _context.SaveChanges();
         }
      }

      public async Task<IEnumerable<TemperatureResult>> GetAllAsync()
      {
         return await _context.TemperatureResults.ToArrayAsync();
      }

      public async Task<TemperatureResult?> GetTemperatureResultAsync(int cityId)
      {
         return await _context.TemperatureResults.FirstOrDefaultAsync(t => t.TemperatureResultId == cityId);
      }

      public Task<int> SaveChangesAsync()
      {
         return _context.SaveChangesAsync();
      }

      public EntityEntry<TemperatureResult> UpdateAsync(TemperatureResult temperatureResult)
      {
         return _context.TemperatureResults.Update(temperatureResult);
      }
   }
}