using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.DeleteHit
{
    public class DeleteHitCommand : BaseCommand, IRequest<Unit>
    {
        public Guid HitId { get; set; }
        
    }
}