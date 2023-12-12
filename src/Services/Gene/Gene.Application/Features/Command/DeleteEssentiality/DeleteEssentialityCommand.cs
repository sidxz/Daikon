
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteEssentiality
{
    public class DeleteEssentialityCommand : BaseCommand, IRequest<Unit>
    {
        public Guid GeneId { get; set; }
        public Guid EssentialityId { get; set; }
    }
}