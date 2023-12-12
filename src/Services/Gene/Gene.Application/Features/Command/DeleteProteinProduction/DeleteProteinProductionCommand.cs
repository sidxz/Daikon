
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteProteinProduction
{
    public class DeleteProteinProductionCommand : BaseCommand, IRequest<Unit>
    {
        public Guid GeneId { get; set; }
        public Guid ProteinProductionId { get; set; }
    }
}