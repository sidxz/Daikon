
using Daikon.Events.Gene;

namespace Gene.Application.Query.Handlers
{
    public interface IGeneEventHandler
    {
        Task OnEvent(GeneCreatedEvent @event);
        Task OnEvent(GeneUpdatedEvent @event);
        Task OnEvent(GeneDeletedEvent @event);
        
    }
}