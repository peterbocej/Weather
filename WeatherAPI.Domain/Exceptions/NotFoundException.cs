using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherAPI.Domain.Exceptions
{
   public class NotFoundException : BaseException
   {
      public NotFoundException() : base()
      { }
      public NotFoundException(string message) : base(message)
      { }
      public NotFoundException(string message, Exception innerException) : base(message, innerException)
      { }
   }
}
