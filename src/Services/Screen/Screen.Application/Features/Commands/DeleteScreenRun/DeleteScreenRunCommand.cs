using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.DeleteScreenRun
{
    public class DeleteScreenRunCommand : BaseCommand, IRequest<Unit>
    {
        public Guid ScreenRunId { get; set; }
    }
}