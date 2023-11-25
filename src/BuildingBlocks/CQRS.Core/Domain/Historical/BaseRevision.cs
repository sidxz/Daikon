using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Domain.Historical
{
    public abstract class BaseRevision
    {
        public Guid Id { get; set; }
    }
}