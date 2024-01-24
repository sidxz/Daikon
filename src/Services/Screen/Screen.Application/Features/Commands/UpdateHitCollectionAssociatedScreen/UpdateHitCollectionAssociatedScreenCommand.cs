
using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.UpdateHitCollectionAssociatedScreen
{
    public class UpdateHitCollectionAssociatedScreenCommand : BaseCommand, IRequest<Unit>
    {
        public Guid ScreenId { get; set; }
    }
}