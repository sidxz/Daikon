using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public abstract class BaseEntity : DocMetadata
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsModified { get; set; }
        public bool IsArchived { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDraft { get; set; }

        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

    }
}