
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitUpdatedEvent : BaseEvent
    {
        public HitUpdatedEvent() : base(nameof(HitUpdatedEvent))
        {
            
        }

        public Guid HitId { get; set; }
        public DVariable<string>? LibrarySource { get; set; }
        public DVariable<string>? Source { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? MIC { get; set; }
        public DVariable<string>? MICUnit { get; set; }
        public DVariable<string>? MICCondition { get; set; }
        public DVariable<string>? IC50 { get; set; }
        public DVariable<string>? IC50Unit { get; set; }
        public DVariable<int>? ClusterGroup { get; set; }
        public DVariable<string>? Notes { get; set; }

        public DVariable<int>? Positive { get; set; }
        public DVariable<int>? Neutral { get; set; }
        public DVariable<int>? Negative { get; set; }
        public DVariable<bool>? IsVotingAllowed { get; set; }
    }
}