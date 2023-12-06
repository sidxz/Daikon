using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Exceptions 
{
    public class ResourceCannotBeDeletedException : Exception
    {
        // Constructor that accepts a string ID
        public ResourceCannotBeDeletedException(string resourceName, string id, string message) 
            : base($"ResourceCannotBeDeletedException: The requested resource {resourceName} with identifier {id} cannot be deleted:  {message}")
        {
        }

        // Constructor that accepts a Guid ID
        public ResourceCannotBeDeletedException(string resourceName, Guid id, string message) 
            : base($"ResourceCannotBeDeletedException: The requested resource {resourceName} with Guid {id} cannot be deleted:  {message}")
        {
        }
    }
}