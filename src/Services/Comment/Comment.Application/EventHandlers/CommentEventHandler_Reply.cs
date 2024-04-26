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
            
            
            // Set Ids (swap)
            reply.Id = @event.ReplyId;
            reply.CommentId = @event.Id;

            try
            {
                await _commentReplyRepository.Create(reply);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "Error occurred while creating comment reply run for CommentReplyAddedEvent", ex);
            }
        }

        public async Task OnEvent(CommentReplyUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentReplyUpdatedEvent: CommentId {Id}, CommentReplyId {ReplyId}", @event.Id, @event.ReplyId);

            var existingCommentReply = await _commentReplyRepository.ReadById(@event.ReplyId);

           
            var reply = _mapper.Map<Domain.Entities.CommentReply>(existingCommentReply);
            _mapper.Map(@event, reply);

            reply.Id = @event.ReplyId;
            reply.CommentId = @event.Id;

             // Preserve the original creation date and creator
            reply.CreatedById = existingCommentReply.CreatedById;
            reply.DateCreated = existingCommentReply.DateCreated;

            try
            {
                await _commentReplyRepository.Update(reply);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating comment reply {@event.ReplyId} for CommentReplyUpdatedEvent", ex);
            }
        }

        public async Task OnEvent(CommentReplyDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentReplyDeletedEvent: {ReplyId}", @event.ReplyId);
            
            try
            {
                await _commentReplyRepository.Delete(@event.ReplyId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting comment reply {@event.ReplyId} for CommentReplyDeletedEvent", ex);
            }
        }
        
    }
}