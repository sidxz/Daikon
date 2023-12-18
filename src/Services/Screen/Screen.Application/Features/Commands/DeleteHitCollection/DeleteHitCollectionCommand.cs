using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.DeleteHitCollection
{
    public class DeleteHitCollectionCommand : BaseCommand, IRequest<Unit>
    {
        public Guid HitCollectionId { get; set; }
        public Guid ScreenId { get; set; }

        public required string Name { get; set; }

        public required string HitCollectionType { get; set; }
    }
}