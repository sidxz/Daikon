using CQRS.Core.Domain;
using Daikon.Events.Comment;

namespace Comment.Domain.Aggregates
{
    public partial class CommentAggregate: AggregateRoot
    {
        private bool _active;
        private Guid _resourceId;

        private string _Topic;

        public CommentAggregate()
        {

        }

        /* New Comment */

        public CommentAggregate(CommentCreatedEvent CommentCreatedEvent)
        {
            _active = true;
            _id = CommentCreatedEvent.Id;
            _resourceId = CommentCreatedEvent.ResourceId;
            _Topic = CommentCreatedEvent.Topic;

            RaiseEvent(CommentCreatedEvent);
            
        }

        public void Apply(CommentCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _resourceId = @event.ResourceId;
            _Topic = @event.Topic;
        }

        /* Update Comment */

        public void UpdateComment(CommentUpdatedEvent CommentUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }

            // CommentUpdatedEvent doesn't allow resourceId to be changed.
            CommentUpdatedEvent.ResourceId = _resourceId;
            CommentUpdatedEvent.Topic = _Topic;

            RaiseEvent(CommentUpdatedEvent);
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Delete Comment */

        public void DeleteComment(CommentDeletedEvent CommentDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is already deleted.");
            }

            RaiseEvent(CommentDeletedEvent);
        }

        public void Apply(CommentDeletedEvent @event)
        {
            _active = false;
        }
    }
}