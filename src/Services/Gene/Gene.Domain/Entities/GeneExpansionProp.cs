using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class GeneExpansionProp : ExpansionProp
    {
        public Guid GeneId { get; set; }
    }
}