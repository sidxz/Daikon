using System;
using System.Collections.Generic;

using Daikon.Events.MLogix;

namespace MLogix.Application.EventHandlers
{
    public interface IMLogixEventHandler
    {
        Task OnEvent(MoleculeCreatedEvent @event);
        Task OnEvent(MoleculeUpdatedEvent @event);
        Task OnEvent(MoleculeDisclosedEvent @event);
        Task OnEvent(MoleculeDeletedEvent @event);
        Task OnEvent(MoleculeNuisancePredictedEvent @event);
    }
}