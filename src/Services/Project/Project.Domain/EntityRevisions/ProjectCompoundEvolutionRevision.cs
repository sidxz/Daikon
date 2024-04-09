
using CQRS.Core.Domain.Historical;

namespace Project.Domain.EntityRevisions
{
    public class ProjectCompoundEvolutionRevision : BaseVersionEntity
    {
        public DVariableHistory<DateTime>? EvolutionDate { get; set; }
        public DVariableHistory<string>? Stage { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
        public DVariableHistory<string>? MIC { get; set; }
        public DVariableHistory<string>? MICUnit { get; set; }
        public DVariableHistory<string>? IC50 { get; set; }
        public DVariableHistory<string>? IC50Unit { get; set; }


        /* Molecule */
        public DVariableHistory<string> RequestedSMILES { get; set; }
        public DVariableHistory<bool> IsStructureDisclosed { get; set; }
    }
}