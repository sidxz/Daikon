using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gene.Application.Features.Queries.GetStrainsList
{
    public class StrainsListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Organism { get; set; }

    }
}