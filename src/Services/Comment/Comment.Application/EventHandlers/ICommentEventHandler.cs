using Daikon.Events.Comment;

namespace Comment.Application.EventHandlers
{
    public interface ICommentEventHandler
    {
        Task OnEvent(CommentCreatedEvent @event);
        Task OnEvent(CommentUpdatedEvent @event);
        Task OnEvent(CommentDeletedEvent @event);
        
        Task OnEvent(CommentReplyAddedEvent @event);
        Task OnEvent(CommentReplyUpdatedEvent @event);
        Task OnEvent(CommentReplyDeletedEvent @event);
        
    }
}