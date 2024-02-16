using CQRS.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Daikon.Events.Comment;


namespace Comment.Application.EventHandlers
{
    public partial class CommentEventHandler : ICommentEventHandler
    {
        public async Task OnEvent(CommentReplyAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentReplyAddedEvent: CommentId {Id}, CommentReplyId {ReplyId}", @event.Id, @event.ReplyId);
            
            var reply = _mapper.Map<Domain.Entities.CommentReply>(@event);
            
            
            reply.Id = @event.ReplyId;
            reply.CommentId = @event.Id;
            reply.DateCreated = DateTime.UtcNow;
            reply.IsModified = false;

            try
            {
                await _commentReplyRepository.CreateCommentReply(reply);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "Error occurred while creating comment reply run for CommentReplyAddedEvent", ex);
            }
        }

        public async Task OnEvent(CommentReplyUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentReplyUpdatedEvent: CommentId {Id}, CommentReplyId {ReplyId}", @event.Id, @event.ReplyId);
            var existingCommentReply = await _commentReplyRepository.ReadCommentReplyById(@event.ReplyId);

            if (existingCommentReply == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating comment reply {@event.ReplyId} for CommentReplyUpdatedEvent", new Exception("Comment reply not found"));
            }

            var reply = _mapper.Map<Domain.Entities.CommentReply>(@event);
            reply.Id = @event.ReplyId;
            reply.CommentId = @event.Id;
            reply.DateCreated = existingCommentReply.DateCreated;
            reply.IsModified = true;

            try
            {
                await _commentReplyRepository.UpdateCommentReply(reply);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating comment reply {@event.ReplyId} for CommentReplyUpdatedEvent", ex);
            }
        }

        public async Task OnEvent(CommentReplyDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentReplyDeletedEvent: {Id}", @event.Id);
            
            try
            {
                await _commentReplyRepository.DeleteCommentReply(@event.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting comment reply {@event.ReplyId} for CommentReplyDeletedEvent", ex);
            }
        }
        
    }
}