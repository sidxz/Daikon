
using CQRS.Core.Domain.Historical;

namespace HitAssessment.Domain.EntityRevisions
{
    public class HaCompoundEvolutionRevision : BaseVersionEntity
    {
        public DVariableHistory<DateTime>? EvolutionDate { get; set; }
        public DVariableHistory<string>? Stage { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
        public DVariableHistory<string>? MIC { get; set; }
        public DVariableHistory<string>? MICUnit { get; set; }
        public DVariableHistory<string>? IC50 { get; set; }
        public DVariableHistory<string>? IC50Unit { get; set; }
    }
}