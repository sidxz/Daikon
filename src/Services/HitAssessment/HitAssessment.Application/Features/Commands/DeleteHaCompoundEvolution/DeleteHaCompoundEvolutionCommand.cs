
using CQRS.Core.Command;
using MediatR;

namespace HitAssessment.Application.Features.Commands.DeleteHaCompoundEvolution
{
    public class DeleteHaCompoundEvolutionCommand : BaseCommand, IRequest<Unit>
    {

    }
}