using CQRS.Core.Command;
using MediatR;
using Screen.Application.Features.Commands.NewHit;
using Daikon.Shared.VM.Screen;
namespace Screen.Application.Features.Commands.NewHitBatch
{
    public class RegisterHitBatchCommand : BaseCommand, IRequest<List<HitVM>>
    {
        public List<NewHitCommand> Commands { get; set; } = [];
    }
}
