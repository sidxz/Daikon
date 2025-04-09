using Daikon.Events.Comment;
using Comment.Domain.Entities;
using Daikon.EventStore.Aggregate;

namespace Comment.Domain.Aggregates
{
    public partial class CommentAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, CommentReply> _replies = [];

        public void AddCommentReply(CommentReplyAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }

            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ReplyId == Guid.Empty)
            {
                throw new InvalidOperationException("Reply Id cannot be empty.");
            }

            if (_replies.ContainsKey(@event.ReplyId))
            {
                throw new Exception("Comment Reply already exists.");
            }

            if (string.IsNullOrWhiteSpace(@event.Body))
            {
                throw new InvalidOperationException($" The reply body cannot be null or whitespace");
            }


            RaiseEvent(@event);
        }

        public void Apply(CommentReplyAddedEvent @event)
        {
            // Store important parameters necessary for the screen aggregate to run
            _replies.Add(@event.ReplyId, new CommentReply()
            {
                CommentId = @event.Id,
                Body = @event.Body,
                Tags = @event.Tags,
                Mentions = @event.Mentions,
                Subscribers = @event.Subscribers
            });
        }

        public void UpdateCommentReply(CommentReplyUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ReplyId == Guid.Empty)
            {
                throw new InvalidOperationException("Reply Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(@event.Body))
            {
                throw new InvalidOperationException($" The reply body cannot be null or whitespace");
            }

            if (!_replies.ContainsKey(@event.ReplyId))
            {
                throw new Exception("Comment Reply does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(CommentReplyUpdatedEvent @event)
        {
            _replies[@event.ReplyId].Body = @event.Body;
            _replies[@event.ReplyId].Tags = @event.Tags;
            _replies[@event.ReplyId].Mentions = @event.Mentions;
            _replies[@event.ReplyId].Subscribers = @event.Subscribers;
        }

        public void DeleteCommentReply(CommentReplyDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }
            if (@event.ReplyId == Guid.Empty)
            {
                throw new InvalidOperationException("Reply Id cannot be empty.");
            }

            if (!_replies.ContainsKey(@event.ReplyId))
            {
                throw new Exception("Comment Reply does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(CommentReplyDeletedEvent @event)
        {
            _replies.Remove(@event.ReplyId);
        }
        
    }
}