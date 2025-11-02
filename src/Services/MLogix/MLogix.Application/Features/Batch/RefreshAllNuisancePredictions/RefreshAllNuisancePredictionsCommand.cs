
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Batch.RefreshAllNuisancePredictions
{
    public class RefreshAllNuisancePredictionsCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}