
using Daikon.Shared.VM.MLogix;

namespace Project.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewProjectCompoundEvolutionResDTO
    {
        public Guid Id { get; set; }
        public Guid MoleculeId { get; set; }
        public MoleculeVM Molecule { get; set; }
        
    }
}