using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Horizon.Domain.Genes
{
    public class Gene : BaseEntity
    {
         public string AccessionNumber { get; set; }
        
        public string? Name { get; set; }
        public string? Function { get; set; }
        public string? Product { get; set; }
        public string? FunctionalCategory { get; set; }
    }
}