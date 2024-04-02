
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.HitAssessment
{
    public class HaCompoundEvolutionAddedEvent : BaseEvent
    {
        public HaCompoundEvolutionAddedEvent() : base(nameof(HaCompoundEvolutionAddedEvent))
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
        public DVariable<string> RequestedSMILES { get; set; }
        public DVariable<bool> IsStructureDisclosed { get; set; }
        public Guid MoleculeId { get; set; }
    }
}