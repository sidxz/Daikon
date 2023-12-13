
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
        
        Task OnEvent(GeneProteinActivityAssayAddedEvent @event);
        Task OnEvent(GeneProteinActivityAssayUpdatedEvent @event);
        Task OnEvent(GeneProteinActivityAssayDeletedEvent @event);

        Task OnEvent(GeneHypomorphAddedEvent @event);
        Task OnEvent(GeneHypomorphUpdatedEvent @event);
        Task OnEvent(GeneHypomorphDeletedEvent @event);

        Task OnEvent(GeneCrispriStrainAddedEvent @event);
        Task OnEvent(GeneCrispriStrainUpdatedEvent @event);
        Task OnEvent(GeneCrispriStrainDeletedEvent @event);
        
        Task OnEvent(GeneResistanceMutationAddedEvent @event);
        Task OnEvent(GeneResistanceMutationUpdatedEvent @event);
        Task OnEvent(GeneResistanceMutationDeletedEvent @event);
        
    }
}