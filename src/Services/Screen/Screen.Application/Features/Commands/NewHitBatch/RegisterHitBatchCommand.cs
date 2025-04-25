using CQRS.Core.Command;
using MediatR;
using Screen.Application.Features.Commands.NewHit;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Commands.NewHitBatch
{
    public class RegisterHitBatchCommand : BaseCommand, IRequest<List<HitVM>>
    {
        public List<NewHitCommand> Commands { get; set; } = [];
    }
}
