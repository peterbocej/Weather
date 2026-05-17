using Microsoft.EntityFrameworkCore;
using WeatherAPI.Domain.Models;

namespace WeatherAPI.Infrastructure.Data
{
   public class CacheDbContext : DbContext
   {
      public CacheDbContext(DbContextOptions<CacheDbContext> options) : base(options)
      {
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
            new TemperatureResult { TemperatureResultId = 1, City = "Bratislava" },
            new TemperatureResult { TemperatureResultId = 2, City = "Praha" },
            new TemperatureResult { TemperatureResultId = 3, City = "Budapest" },
            new TemperatureResult { TemperatureResultId = 4, City = "Vienna" }
         );
      }
   }
}
