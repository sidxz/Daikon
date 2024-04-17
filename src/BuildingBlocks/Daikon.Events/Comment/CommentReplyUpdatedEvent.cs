
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Comment
{
    public class CommentReplyUpdatedEvent: BaseEvent
    {
        public CommentReplyUpdatedEvent() : base(nameof(CommentReplyUpdatedEvent))
        {

        }
        
        public Guid CommentId { get; set; }
        public Guid ReplyId { get; set; }
        public DVariable<string> Body { get; set; }
        public HashSet<string>? Tags { get; set; }
        public HashSet<Guid>? Mentions { get; set; }
        public HashSet<Guid>? Subscribers { get; set; }

    }
}