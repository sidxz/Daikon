
using CQRS.Core.Command;
using MediatR;

namespace Project.Application.Features.Commands.DeleteProjectCompoundEvolution
{
    public class DeleteProjectCompoundEvolutionCommand : BaseCommand, IRequest<Unit>
    {
        public Guid CompoundEvolutionId { get; set; }
    }
}