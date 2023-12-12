
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteHypomorph
{
    public class DeleteHypomorphCommand : BaseCommand, IRequest<Unit>
    {
        public Guid GeneId { get; set; }
        public Guid HypomorphId { get; set; }
    }
}