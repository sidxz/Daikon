

using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.RenameScreen
{
    public class RenameScreenCommand : BaseCommand, IRequest<Unit>
    {
        public required string Name { get; set; }
    }
}