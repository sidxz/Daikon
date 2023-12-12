
using Daikon.Events.Gene;

namespace Gene.Application.Query.EventHandlers
{
    public interface IGeneEventHandler
    {
        Task OnEvent(GeneCreatedEvent @event);
        Task OnEvent(GeneUpdatedEvent @event);
        Task OnEvent(GeneDeletedEvent @event);

        Task OnEvent(GeneEssentialityAddedEvent @event);
        Task OnEvent(GeneEssentialityUpdatedEvent @event);
        Task OnEvent(GeneEssentialityDeletedEvent @event);

        Task OnEvent(GeneProteinProductionAddedEvent @event);
        Task OnEvent(GeneProteinProductionUpdatedEvent @event);
        Task OnEvent(GeneProteinProductionDeletedEvent @event);
        
    }
}