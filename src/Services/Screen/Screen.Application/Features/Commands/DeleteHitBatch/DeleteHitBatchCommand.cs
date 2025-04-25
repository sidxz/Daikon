using MediatR;
using Screen.Application.Features.Commands.DeleteHit;

namespace Screen.Application.Features.Commands.DeleteHitBatch
{
    public class DeleteHitBatchCommand : IRequest<Unit>
    {
        public Guid RequestorUserId { get; set; }
        public List<DeleteHitCommand> Commands { get; set; } = new();
    }
}
