using Microsoft.EntityFrameworkCore;
using Weather.Domain.Models.Security;

namespace Weather.Infrastructure.Data
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
