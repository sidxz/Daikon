using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gene.Application.Features.Queries.GetGenesList
{
    public class GenesListVM
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public string Product { get; set; }
        public string FunctionalCategory { get; set; }

    }
}