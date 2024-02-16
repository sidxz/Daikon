using CQRS.Core.Event;

namespace Daikon.Events.Comment
{
    public class CommentUpdatedEvent : BaseEvent
    {
        public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
        {

        }

        public Guid ResourceId { get; set; }
        public string? Reference { get; set; }
        public string? Section { get; set; }
        public string Topic { get; set; }
        public string? Description { get; set; }
        public string? PostedBy { get; set; }
        
    }
}