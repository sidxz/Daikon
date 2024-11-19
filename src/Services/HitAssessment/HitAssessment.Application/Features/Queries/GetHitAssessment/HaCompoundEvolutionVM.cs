
using CQRS.Core.Domain;
using Daikon.Shared.VM.MLogix;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment
{
    public class HaCompoundEvolutionVM : VMMeta
    {
        public Guid Id { get; set; }
        public Guid HitAssessmentId { get; set; }
        public object EvolutionDate { get; set; }
        public object Stage { get; set; }
        public object Notes { get; set; }
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