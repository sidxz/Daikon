
using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.UpdateScreenAssociatedTargets
{
    public class UpdateScreenAssociatedTargetsCommand : BaseCommand, IRequest<Unit>
    {
        public Dictionary<string, string>? AssociatedTargets { get; set; }
    }
}