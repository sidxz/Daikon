
using CQRS.Core.Domain;

namespace HitAssessment.Domain.Entities
{
    public class HaCompoundEvolution : BaseEntity
    {
        public Guid HitAssessmentId { get; set; }
        public DVariable<DateTime>? EvolutionDate { get; set; }
        public DVariable<string>? Stage { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? MIC { get; set; }
        public DVariable<string>? MICUnit { get; set; }
        public DVariable<string>? IC50 { get; set; }
        public DVariable<string>? IC50Unit { get; set; }


         /* Molecule */
        public DVariable<string> RequestedSMILES { get; set; }
        public DVariable<bool> IsStructureDisclosed { get; set; }
        public Guid MoleculeId { get; set; }
        
    }
}