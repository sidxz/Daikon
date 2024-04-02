using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Horizon.Domain.Genes
{
    public class Gene : GraphEntity
    {
        public string GeneId { get; set; }
        public string AccessionNumber { get; set; }
        public string StrainId { get; set; }
        public string? Name { get; set; }
        public string? Function { get; set; }
        public string? Product { get; set; }
        public string? FunctionalCategory { get; set; }
    }
}