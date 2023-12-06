using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Exceptions
{
    public class DuplicateEntityRequestException : Exception
    {
        public DuplicateEntityRequestException(string commandName, string message)
            : base($"DuplicateEntityRequestException at: {commandName} : {message}")
        {
        }
    }
}