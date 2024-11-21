
using Daikon.Events.DocuStore;

namespace DocuStore.Application.EventHandlers
{
    public interface IParsedDocEventHandler
    {
        Task OnEvent(ParsedDocAddedEvent @event);
        Task OnEvent(ParsedDocUpdatedEvent @event);
        Task OnEvent(ParsedDocDeletedEvent @event);

    }
}