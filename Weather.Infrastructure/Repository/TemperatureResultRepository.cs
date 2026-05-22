using Weather.Domain.Models;
using Weather.Infrastructure.Data;

namespace Weather.Infrastructure.Repository
{
   public interface ITemperatureResultRepository : IBaseRepository<TemperatureResult>
   {
   }
   public class TemperatureResultRepository(CacheDbContext context) : BaseRepository<TemperatureResult>(context), ITemperatureResultRepository
   {
   }
}