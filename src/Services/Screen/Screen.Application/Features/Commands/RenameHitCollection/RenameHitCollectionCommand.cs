
using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.RenameHitCollection
{
    public class RenameHitCollectionCommand : BaseCommand, IRequest<Unit>
    {
        public required string Name { get; set; }
        
    }
}