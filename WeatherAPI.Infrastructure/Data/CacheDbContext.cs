using Microsoft.EntityFrameworkCore;
using WeatherAPI.Domain;
using WeatherAPI.Domain.Models;

namespace WeatherAPI.Infrastructure.Data
{
   public class CacheDbContext : DbContext
   {
      private readonly AppSettings _settings;
      public CacheDbContext(DbContextOptions<CacheDbContext> options, AppSettings settings) : base(options)
      {
         _settings = settings;
      }

      public DbSet<TemperatureResult> TemperatureResults { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);
         SeedData(modelBuilder);
      }

      private void SeedData(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<TemperatureResult>().HasData(
            _settings.Cities.Select(c => new TemperatureResult
            {
               Id = c.Id,
               City = c.Name
            }).ToArray()
         );
      }
   }
}
