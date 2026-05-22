using System.Text;

namespace WeatherAPI.Domain.Extensions
{
   public static class ExceptionExtensions
   {
      public static string GetFullMessage(this Exception exception, string separator = "\n")
      {
         var sb = new StringBuilder();
         sb.Append(exception.Message);
         if (exception.InnerException != null)
         {
            sb.Append(separator);
            sb.Append(exception.InnerException.GetFullMessage(separator));
         }
         return sb.ToString();
      }
   }
}
