using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneVM
    {
        public Guid Id { get; set; }
         public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
        public string Product { get; set; }
        public string FunctionalCategory { get; set; }

    }
}