
using Daikon.Events.MLogix;

namespace Horizon.Application.Handlers
{
    public interface IMLogixEventHandler
    {
        Task OnEvent(MoleculeCreatedEvent @event);
        Task OnEvent(MoleculeUpdatedEvent @event);
        Task OnEvent(MoleculeDisclosedEvent @event);
    }
}