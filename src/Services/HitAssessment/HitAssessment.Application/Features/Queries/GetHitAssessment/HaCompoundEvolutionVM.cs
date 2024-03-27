
using CQRS.Core.Domain;

namespace HitAssessment.Application.Features.Queries.GetHitAssessment
{
    public class HaCompoundEvolutionVM : DocMetadata
    {
        //public Guid HitAssessmentId { get; set; }
        public Guid CompoundId { get; set; }
        public object EvolutionDate { get; set; }
        public object Stage { get; set; }
        public object Notes { get; set; }
        public object MIC { get; set; }
        public object MICUnit { get; set; }
        public object IC50 { get; set; }
        public object IC50Unit { get; set; }
        public MoleculeVM Molecule { get; set; }
    }
}