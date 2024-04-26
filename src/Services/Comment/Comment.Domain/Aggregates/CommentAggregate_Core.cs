using CQRS.Core.Domain;
using Daikon.Events.Comment;

namespace Comment.Domain.Aggregates
{
    public partial class CommentAggregate: AggregateRoot
    {
        private bool _active;
        private Guid? _resourceId;
        private string _topic;

        public CommentAggregate()
        {

        }

        /* New Comment */

        public CommentAggregate(CommentCreatedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Comment Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(@event.Topic))
            {
                throw new InvalidOperationException($" Topic cannot be null or whitespace");
            }
            _active = true;
            _id = @event.Id;
            _resourceId = @event.ResourceId;
            _topic = @event.Topic;

            RaiseEvent(@event);
            
        }

        public void Apply(CommentCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _resourceId = @event.ResourceId;
            _topic = @event.Topic;
        }

        /* Update Comment */

        public void UpdateComment(CommentUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Comment Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(@event.Topic))
            {
                throw new InvalidOperationException($" Topic cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _resourceId = @event.ResourceId;
            _topic = @event.Topic;
        }

        /* Delete Comment */

        public void DeleteComment(CommentDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is already deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Comment Id cannot be empty.");
            }

            RaiseEvent(@event);
        }

        public void Apply(CommentDeletedEvent @event)
        {
            _active = false;
        }
    }
}