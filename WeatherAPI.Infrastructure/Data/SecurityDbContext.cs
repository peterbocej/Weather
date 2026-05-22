using Microsoft.EntityFrameworkCore;
using WeatherAPI.Domain.Models.Security;

namespace WeatherAPI.Infrastructure.Data
{
   public class SecurityDbContext : DbContext
   {
      public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
      { }

      public DbSet<User> Users { get; set; }
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);
         modelBuilder.Entity<User>()
            .HasIndex(u => u.UserEmail).IsUnique();
      }
   }
}
