using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Application.Features.Queries.FindRelatedTarget
{
    public class RelatedTargetVM
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public string Name { get; set; }
        public string TargetType { get; set; }
    }
}