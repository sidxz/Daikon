using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Exceptions
{
    public class EventHandlerException : Exception
    {
        public EventHandlerException(string eventHandlerName, string message, Exception innerException)
            : base($"EventHandlerException at: {eventHandlerName} : {message}", innerException)
        {
        }
    }
}