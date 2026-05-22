using Microsoft.EntityFrameworkCore;
using WeatherAPI.Domain.Models.Security;
using WeatherAPI.Infrastructure.Data;

namespace WeatherAPI.Infrastructure.Repository
{
   public interface IUsersRepository : IBaseRepository<User>
   {
      Task<User?> GetByUsernameAsync(string username);
   }
   public class UsersRepository(SecurityDbContext context) : BaseRepository<User>(context), IUsersRepository
   {
      public async Task<User?> GetByUsernameAsync(string username)
      {
         return await base.GetDbSetAsync().FirstOrDefaultAsync(u => u.UserName.Equals(username));
      }
   }
}
