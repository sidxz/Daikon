
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Command.UpdateTargetAssociatedGenes
{
    public class UpdateTargetAssociatedGenesCommand : BaseCommand, IRequest<Unit>
    {
        
        public Dictionary<string, string> AssociatedGenes { get; set; }
        
    }
}