using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class GeneRevision : BaseVersionEntity
    {
        
        // public DVariableHistory<string> Name { get; set; }
        public DVariableHistory<string> Function { get; set; }
        public DVariableHistory<string> Product { get; set; }
        public DVariableHistory<string> FunctionalCategory { get; set; }
        
    }
}