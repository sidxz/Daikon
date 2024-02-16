using CQRS.Core.Domain;
using Daikon.Events.Comment;
using Comment.Domain.Entities;

namespace Comment.Domain.Aggregates
{
    public partial class CommentAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, CommentReply> _replies = [];

        public void AddCommentReply(CommentReplyAddedEvent CommentReplyAddedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }

            if (_replies.ContainsKey(CommentReplyAddedEvent.ReplyId))
            {
                throw new Exception("Comment Reply already exists.");
            }
            RaiseEvent(CommentReplyAddedEvent);
        }

        public void Apply(CommentReplyAddedEvent @event)
        {
            // Store important parameters necessary for the screen aggregate to run
            _replies.Add(@event.ReplyId, new CommentReply()
            {
                CommentId = @event.Id,
                ResourceId = @event.ResourceId,
                Body = @event.Body,
                PostedBy = @event.PostedBy,
            });
        }

        public void UpdateCommentReply(CommentReplyUpdatedEvent CommentReplyUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }

            if (!_replies.ContainsKey(CommentReplyUpdatedEvent.ReplyId))
            {
                throw new Exception("Comment Reply does not exist.");
            }

            RaiseEvent(CommentReplyUpdatedEvent);
        }

        public void Apply(CommentReplyUpdatedEvent @event)
        {
            // Update the existing CommentReply identified by @event.CommentReplyId without creating a new one
            // Store important parameters necessary for the screen aggregate to run
            _replies[@event.ReplyId].Body = @event.Body;

        }

        public void DeleteCommentReply(CommentReplyDeletedEvent CommentReplyDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Comment is deleted.");
            }

            if (!_replies.ContainsKey(CommentReplyDeletedEvent.ReplyId))
            {
                throw new Exception("Comment Reply does not exist.");
            }

            RaiseEvent(CommentReplyDeletedEvent);
        }

        public void Apply(CommentReplyDeletedEvent @event)
        {
           
            _replies.Remove(@event.ReplyId);
        }
        
    }
}