using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.DeleteScreenRun
{
    public class DeleteScreenRunCommand: BaseCommand, IRequest<Unit>
    {
        public Guid ScreenId { get; set; }
        public Guid ScreenRunId { get; set; }
    }
}