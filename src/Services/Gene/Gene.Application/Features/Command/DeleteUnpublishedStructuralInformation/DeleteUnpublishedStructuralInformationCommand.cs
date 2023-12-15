
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteUnpublishedStructuralInformation
{
    public class DeleteUnpublishedStructuralInformationCommand : BaseCommand, IRequest<Unit>
    {
        public Guid GeneId { get; set; }
        public Guid UnpublishedStructuralInformationId { get; set; }
    }
}