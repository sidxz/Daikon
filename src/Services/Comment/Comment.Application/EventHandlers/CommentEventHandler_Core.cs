using AutoMapper;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Comment.Application.Contracts.Persistence;
using Daikon.Events.Comment;

namespace Comment.Application.EventHandlers
{
    public partial class CommentEventHandler : ICommentEventHandler
    {

        private readonly ICommentRepository _commentRepository;

        private readonly ICommentReplyRepository _commentReplyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentEventHandler> _logger;

        public CommentEventHandler(ICommentRepository commentRepository, IMapper mapper, ILogger<CommentEventHandler> logger)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task OnEvent(CommentCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentCreatedEvent: {Id}", @event.Id);
            var comment = _mapper.Map<Domain.Entities.Comment>(@event);
            comment.Id = @event.Id;
            comment.DateCreated = DateTime.UtcNow;
            comment.IsModified = false;

            try
            {
                await _commentRepository.CreateComment(comment);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "CommentCreatedEvent Error creating comment", ex);
            }
        }

        public async Task OnEvent(CommentUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentUpdatedEvent: {Id}", @event.Id);
            var existingComment = await _commentRepository.ReadCommentById(@event.Id);

            if (existingComment == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"CommentUpdatedEvent Error updating comment {@event.Id}", new Exception("Comment not found"));
            }

            var comment = _mapper.Map<Domain.Entities.Comment>(@event);
            comment.DateCreated = existingComment.DateCreated;
            comment.DateModified = DateTime.UtcNow;
            comment.IsModified = true;

            try
            {
                await _commentRepository.UpdateComment(comment);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"CommentUpdatedEvent Error updating comment {@event.Id}", ex);
            }
        }

        public async Task OnEvent(CommentDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: CommentDeletedEvent: {Id}", @event.Id);
            

            try
            {
                await _commentRepository.DeleteComment(@event.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"CommentDeletedEvent Error deleting comment {@event.Id}", ex);
            }
        }
    }
}