
using CQRS.Core.Event;

namespace Daikon.Events.Comment
{
    public class CommentReplyUpdatedEvent: BaseEvent
    {
        public CommentReplyUpdatedEvent() : base(nameof(CommentReplyUpdatedEvent))
        {

        }
        
        public Guid ReplyId { get; set; }
        public Guid ResourceId { get; set; }
        public string? Body { get; set; }
        public string? PostedBy { get; set; }

    }
}