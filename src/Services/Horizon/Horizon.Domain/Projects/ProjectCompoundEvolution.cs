using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Domain.Projects
{
    public class ProjectCompoundEvolution : GraphEntity
    {
        public string CompoundEvolutionId { get; set; }
        public string ProjectId { get; set; }
        public DateTime? EvolutionDate { get; set; }
        public string? Stage { get; set; }
        public string MoleculeId { get; set; }
    }
}