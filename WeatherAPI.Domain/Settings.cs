using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAPI.Domain
{
   public class Settings
   {
      public WeatherApiServer WeatherApiServer { get; set; }
      public Jwt Jwt { get; set; }
      public Cache Cache { get; set; }
      public City[] Cities { get; private set; }

      public Settings(IConfigurationSection configuration)
      {
         WeatherApiServer = new WeatherApiServer
         {
            Url = configuration["WeatherApiServer:Url"] ?? string.Empty,
            ApiKey = configuration["WeatherApiServer:ApiKey"] ?? string.Empty
         };
         Jwt = new Jwt
         {
            Key = configuration["Jwt:Key"] ?? string.Empty,
            Issuer = configuration["Jwt:Issuer"] ?? string.Empty,
            Audience = configuration["Jwt:Audience"] ?? string.Empty,
            DurationInMinutes = int.TryParse(configuration["Jwt:DurationInMinutes"], out var duration) ? duration : 0
         };
         Cache = new Cache
         {
            Mode = Enum.TryParse<CacheMode>(configuration["Cache:Mode"], out var mode) ? mode : CacheMode.Memory
         };
         Cities = configuration.GetSection("Cities").GetChildren()
            .Select(c => new City
            {
               Id = int.TryParse(c["Id"], out var id) ? id : 0,
               Name = c["Name"] ?? string.Empty
            }).ToArray();
      }
   }

   public class WeatherApiServer
   {
      public string Url { get; set; } = string.Empty;
      public string ApiKey { get; set; } = string.Empty;
   }

   public class Jwt
   {
      public string Key { get; set; } = string.Empty;
      public string Issuer { get; set; } = string.Empty;
      public string Audience { get; set; } = string.Empty;
      public int DurationInMinutes { get; set; }
   }

   public class Cache
   {
      public CacheMode Mode { get; set; }
   }

   public enum CacheMode
   {
      None,
      Memory,
      Database
   }
   public class City
   {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;
   }
}
