using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Exceptions
{
    public class EntityDeletedException : Exception
    {
        public EntityDeletedException(string message) : base(message)
        {
            
        }
    }
}