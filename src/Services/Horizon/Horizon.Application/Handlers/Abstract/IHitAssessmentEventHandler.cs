using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.HitAssessment;

namespace Horizon.Application.Handlers
{
    public interface IHitAssessmentEventHandler
    {
        Task OnEvent(HaCreatedEvent @event);
        Task OnEvent(HaUpdatedEvent @event);
        Task OnEvent(HaDeletedEvent @event);

        Task OnEvent(HaRenamedEvent @event);

        Task OnEvent(HaCompoundEvolutionAddedEvent @event);
        Task OnEvent(HaCompoundEvolutionUpdatedEvent @event);
        Task OnEvent(HaCompoundEvolutionDeletedEvent @event);
    }
}