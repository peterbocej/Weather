using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WeatherAPI.Domain;
using WeatherAPI.Domain.Models;
using WeatherAPI.Infrastructure.Data;

namespace WeatherAPI.Infrastructure.Repository
{
   public interface ITemperatureResultRepository : IBaseRepository<TemperatureResult>
   {
   }
   public class TemperatureResultRepository(CacheDbContext context) : BaseRepository<TemperatureResult>(context), ITemperatureResultRepository
   {
   }
}