
using Daikon.Events.Strains;

namespace Gene.Application.Query.Handlers
{
    public interface IStrainEventHandler
    {
        Task OnEvent(StrainCreatedEvent @event);
        Task OnEvent(StrainUpdatedEvent @event);
        Task OnEvent(StrainDeletedEvent @event);
        
    }
}