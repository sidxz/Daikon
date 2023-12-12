
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteCrispriStrain
{
    public class DeleteCrispriStrainCommand : BaseCommand, IRequest<Unit>
    {
        public Guid GeneId { get; set; }
        public Guid CrispriStrainId { get; set; }
    }
}