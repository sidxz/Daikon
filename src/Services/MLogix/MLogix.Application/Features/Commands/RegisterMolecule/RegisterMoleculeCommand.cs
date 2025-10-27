
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeCommand : BaseCommand, IRequest<RegisterMoleculeResponseDTO>
    {
        public string? Name { get; set; }
        public string SMILES { get; set; } = string.Empty;
        public string? Synonyms { get; set; }
        public string? DisclosureStage { get; set; } = string.Empty;
        public string DisclosureScientist { get; set; } = string.Empty;
        public Guid DisclosureOrgId { get; set; } = Guid.Empty;
        public string DisclosureReason { get; set; } = string.Empty;
        public string DisclosureNotes { get; set; } = string.Empty;
        public string LiteratureReferences { get; set; } = string.Empty;
        public DateTime StructureDisclosedDate { get; set; } = new DateTime();


    }
}