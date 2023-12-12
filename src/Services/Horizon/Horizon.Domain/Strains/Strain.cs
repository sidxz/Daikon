using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Horizon.Domain.Strains
{
    public class Strain : BaseEntity
    {
        public string StrainId { get; set; }
        public string Name { get; set; }
        public string? Organism { get; set; }
        
    }
}