
using CQRS.Core.Event;

namespace Daikon.Events.Project
{
    public class ProjectCompoundEvolutionAddedEvent : BaseEvent
    {
        public ProjectCompoundEvolutionAddedEvent() : base(nameof(ProjectCompoundEvolutionAddedEvent))
        {

        }
        public Guid CompoundEvolutionId { get; set; }
        public Guid CompoundId { get; set; }
        public DateTime? EvolutionDate { get; set; }
        public string? Stage { get; set; }
        public string? Notes { get; set; }
        public string? MIC { get; set; }
        public string? MICUnit { get; set; }
        public string? IC50 { get; set; }
        public string? IC50Unit { get; set; }
    }
}