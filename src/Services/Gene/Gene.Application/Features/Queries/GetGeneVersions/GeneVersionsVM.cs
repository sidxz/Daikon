using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain.Historical;

namespace Gene.Application.Features.Queries.GetGeneVersions
{
    public class GeneVersionsVM
    {   
        public Guid Id { get; set; }
        public DVariableHistory<string> Function { get; set; }
        public DVariableHistory<string> Product { get; set; }
        public DVariableHistory<string> FunctionalCategory { get; set; }
    }
}