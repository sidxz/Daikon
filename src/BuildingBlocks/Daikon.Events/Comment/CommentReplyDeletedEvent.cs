using CQRS.Core.Event;

namespace Daikon.Events.Comment
{
    public class CommentReplyDeletedEvent: BaseEvent
    {
        public CommentReplyDeletedEvent() : base(nameof(CommentReplyDeletedEvent))
        {

        }

        public Guid ReplyId { get; set; }
    }
}