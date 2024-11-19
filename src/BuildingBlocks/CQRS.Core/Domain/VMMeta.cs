using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public abstract class VMMeta : DocMetadata
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public virtual DateTime? PageLastUpdatedDate { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public virtual Guid? PageLastUpdatedUser { get; set; }
    }
}