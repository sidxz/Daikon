using CQRS.Core.Domain;
using Daikon.EventStore.Event;
namespace Daikon.Events.Comment
{
    public class CommentReplyAddedEvent : BaseEvent
    {
        public CommentReplyAddedEvent() : base(nameof(CommentReplyAddedEvent))
        {

        }
        public Guid CommentId { get; set; }
        public Guid ReplyId { get; set; }
        public DVariable<string> Body { get; set; }
        public HashSet<string> Tags { get; set; }
        public HashSet<Guid> Mentions { get; set; }
        public HashSet<Guid> Subscribers { get; set; }
    }
}