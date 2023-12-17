using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.DeleteHit
{
    public class DeleteHitCommand : BaseCommand, IRequest<Unit>
    {
        public Guid HitCollectionId { get; set; }
        public Guid HitId { get; set; }
        
    }
}