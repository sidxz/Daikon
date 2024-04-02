using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Horizon.Domain
{
    public class GraphEntity : BaseEntity
    {
        public string UniId { get; set; }
    }
}