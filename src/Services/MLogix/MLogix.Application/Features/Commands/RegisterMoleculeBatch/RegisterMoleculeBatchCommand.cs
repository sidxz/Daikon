
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{
    public class RegisterMoleculeCommandWithRegId : RegisterMoleculeCommand
    {
        public Guid RegistrationId { get; set; }
    }
    public class RegisterMoleculeBatchCommand : BaseCommand, IRequest<List<RegisterMoleculeResponseDTO>>
    {
        public List<RegisterMoleculeCommandWithRegId> Commands { get; set; } = [];
        public bool PreviewMode { get; set; } = false;
    }
}