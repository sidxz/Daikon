
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeCommand : BaseCommand, IRequest<RegisterMoleculeResponseDTO>
    {
        public string? Name { get; set; }
        public string SMILES { get; set; }
        public string? Synonyms { get; set; }

    }
}