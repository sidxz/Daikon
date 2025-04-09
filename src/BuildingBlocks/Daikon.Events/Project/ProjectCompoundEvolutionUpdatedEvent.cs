
using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace Daikon.Events.Project
{
    public class ProjectCompoundEvolutionUpdatedEvent: BaseEvent
    {
        public ProjectCompoundEvolutionUpdatedEvent() : base(nameof(ProjectCompoundEvolutionUpdatedEvent))
        {

        }
        public Guid CompoundEvolutionId { get; set; }
        public DateTime? EvolutionDate { get; set; }
        public string? Stage { get; set; }
        public string? Notes { get; set; }
        public string? MIC { get; set; }
        public string? MICUnit { get; set; }
        public string? IC50 { get; set; }
        public string? IC50Unit { get; set; }
        public DVariable<bool> IsStructureDisclosed { get; set; }
    }
}