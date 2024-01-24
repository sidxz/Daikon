using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Commands.DeleteHitCollection
{
    public class DeleteHitCollectionCommand : BaseCommand, IRequest<Unit>
    {
        public Guid HitCollectionId { get; set; }
        public Guid ScreenId { get; set; }

        public  string Name { get; set; }

        public  string HitCollectionType { get; set; }
    }
}