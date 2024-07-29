
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Targets
{
    public class TargetToxicologyUpdatedEvent : BaseEvent
    {
        public TargetToxicologyUpdatedEvent() : base(nameof(TargetToxicologyUpdatedEvent))
        {
            Impact = new DVariable<string>() { Value = default! };
            ImpactPriority = new DVariable<bool>() { Value = default! };
            Likelihood = new DVariable<string>() { Value = default! };
            LikelihoodPriority = new DVariable<bool>() { Value = default! };
            Note = new DVariable<string>() { Value = default! };
        }

        public Guid TargetId { get; set; }
        public Guid ToxicologyId { get; set; }
        public required DVariable<string> Topic { get; set; }
        public DVariable<string> Impact { get; set; }
        public DVariable<bool> ImpactPriority { get; set; }
        public DVariable<string> Likelihood { get; set; }
        public DVariable<bool> LikelihoodPriority { get; set; }
        public DVariable<string> Note { get; set; }
        
    }
}