using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Application.Features.Queries.FIndTargetRelations
{
    public class TargetRelationsVM
    {
        public string TargetId { get; set; }
        public string TargetName { get; set; }
        public string HighestRelationship { get; set; }
    }
}