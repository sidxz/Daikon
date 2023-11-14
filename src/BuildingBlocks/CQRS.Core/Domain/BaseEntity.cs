using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsModified { get; set; }
        public bool IsArchived { get; set; }
        public bool IsDraft { get; set; }

    }
}