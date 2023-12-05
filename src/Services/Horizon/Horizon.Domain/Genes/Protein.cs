using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Horizon.Domain.Genes
{
    public class Protein : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Function { get; set; }
    }
}