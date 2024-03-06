
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeCommand : BaseCommand, IRequest<RegisterMoleculeResponseDTO>
    {
        public string? Name { get; set; }
        public string RequestedSMILES { get; set; }
        public List<string>? Synonyms   { get; set; }
        public Dictionary<string, string>? Ids { get; set; }
    }
}