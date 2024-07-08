using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Domain.HitAssessment
{
    public class HACompoundEvolution : GraphEntity
    {
        public string CompoundEvolutionId { get; set; }
        public string HitAssessmentId { get; set; }
        public DateTime? EvolutionDate { get; set; }
        public string? Stage { get; set; }
        public string MoleculeId { get; set; }
    }
}