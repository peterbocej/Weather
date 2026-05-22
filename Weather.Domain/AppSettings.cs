using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Weather.Domain
{
   public class AppSettings
   {
      public WeatherApiServer WeatherApiServer { get; set; } = new WeatherApiServer();
      public Jwt Jwt { get; set; } = new Jwt();
      public Cache Cache { get; set; } = new Cache();
      public IList<City> Cities { get; private set; } = new List<City>();
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
