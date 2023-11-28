using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Domain.Historical
{
    public abstract class BaseVersionEntity
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public bool IsEntityDeleted { get; set; }
    }
}