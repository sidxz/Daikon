
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Command.DeleteTarget
{
    public class DeleteTargetCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}