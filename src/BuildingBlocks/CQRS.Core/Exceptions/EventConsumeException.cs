using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Exceptions
{
    public class EventConsumeException : Exception
    {
        public EventConsumeException(string consumerName, string message, Exception innerException)
            : base($"ConsumerException at: {consumerName} : {message}", innerException)
        {
        }
    }
}