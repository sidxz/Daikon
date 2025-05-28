using Daikon.EventStore.Event;

namespace Daikon.Events.Comment
{
    public class CommentDeletedEvent : BaseEvent
    {
        public CommentDeletedEvent() : base(nameof(CommentDeletedEvent))
        {

        }
    }
}