using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.Gene;

namespace Horizon.Application.Query.Handlers
{
    public interface IGeneEventHandler
    {
        Task OnEvent(GeneCreatedEvent @event);
        Task OnEvent(GeneUpdatedEvent @event);
        Task OnEvent(GeneDeletedEvent @event);
        
    }
}