using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Screens
{
    public class HitCreatedEvent : BaseEvent
    {
        public HitCreatedEvent() : base(nameof(HitCreatedEvent))
        {
            
        }
        public Guid ScreenId { get; set; }
        public string? Name { get; set; }
        public required string Type { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? Owner { get; set; }
    }
}
