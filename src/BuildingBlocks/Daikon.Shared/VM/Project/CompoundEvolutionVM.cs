
using CQRS.Core.Domain;
using Daikon.Shared.VM.MLogix;

namespace Daikon.Shared.VM.Project
{
    public class CompoundEvolutionVM : VMMeta
    {
        public Guid Id { get; set; }
        public Guid HitAssessmentId { get; set; }
        public DateTime EvolutionDate { get; set; }
        public string Stage { get; set; }
        public string Notes { get; set; }
        public object MIC { get; set; }
        public object MICUnit { get; set; }
        public object IC50 { get; set; }
        public object IC50Unit { get; set; }

        public object RequestedSMILES { get; set; }
        public object IsStructureDisclosed { get; set; }
        public Guid MoleculeId { get; set; }
        public MoleculeVM Molecule { get; set; }
    }
}