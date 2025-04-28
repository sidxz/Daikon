using MediatR;
using Screen.Application.Features.Commands.UpdateHit;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Commands.UpdateHitBatch
{
    public class UpdateHitBatchCommand : IRequest<List<HitVM>>
    {
        public Guid RequestorUserId { get; set; }
        public List<UpdateHitCommand> Commands { get; set; } = new();
    }
}
