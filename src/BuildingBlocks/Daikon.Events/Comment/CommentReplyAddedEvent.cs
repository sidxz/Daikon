using CQRS.Core.Event;
namespace Daikon.Events.Comment
{
    public class CommentReplyAddedEvent : BaseEvent
    {
        public CommentReplyAddedEvent() : base(nameof(CommentReplyAddedEvent))
        {

        }
        public Guid ReplyId { get; set; }
        public Guid ResourceId { get; set; }
        public string? Body { get; set; }
        public string? PostedBy { get; set; }

    }
}