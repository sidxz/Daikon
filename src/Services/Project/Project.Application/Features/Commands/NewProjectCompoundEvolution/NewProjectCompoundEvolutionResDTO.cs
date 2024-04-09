
using Project.Application.Features.Queries.GetHitAssessment;
using Project.Application.Features.Queries.GetProject;

namespace Project.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewProjectCompoundEvolutionResDTO
    {
        public Guid Id { get; set; }
        public Guid MoleculeId { get; set; }
        public MoleculeVM Molecule { get; set; }
        
    }
}