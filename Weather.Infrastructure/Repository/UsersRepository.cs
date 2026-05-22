using Microsoft.EntityFrameworkCore;
using Weather.Domain.Models.Security;
using Weather.Infrastructure.Data;

namespace Weather.Infrastructure.Repository
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
