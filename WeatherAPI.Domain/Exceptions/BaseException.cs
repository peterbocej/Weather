using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherAPI.Domain.Exceptions
{
   public abstract class BaseException : Exception
   {
      public BaseException() : base() 
      { }
      public BaseException(string message) : base(message)
      { }
      public BaseException(string message, Exception innerException) : base(message, innerException)
      { }
      public virtual string FullMessage => GetFullMessage();
      public virtual string GetFullMessage(Exception? ex = null, string separator = "\n")
      {
         if (ex == null)
            ex = this;
         var message = new StringBuilder();
         message.Append(ex.Message);
         if (InnerException != null)
         {
            message.Append($"{separator}{GetFullMessage(ex.InnerException, separator)}");
         }
         return message.ToString();
      }
   }

   
}
