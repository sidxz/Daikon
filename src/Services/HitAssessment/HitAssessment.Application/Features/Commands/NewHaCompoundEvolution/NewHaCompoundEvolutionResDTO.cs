
using HitAssessment.Application.Features.Queries.GetHitAssessment;

namespace HitAssessment.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewHaCompoundEvolutionResDTO
    {
        public Guid Id { get; set; }
        public Guid MoleculeId { get; set; }
        public MoleculeVM Molecule { get; set; }
        
    }
}