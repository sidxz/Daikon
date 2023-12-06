using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class Strain : BaseEntity
    {
        public string Name { get; set; }
        public string Organism { get; set; }
        
    }
}