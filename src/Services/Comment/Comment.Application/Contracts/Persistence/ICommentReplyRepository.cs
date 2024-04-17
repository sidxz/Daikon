
namespace Comment.Application.Contracts.Persistence
{
    public interface ICommentReplyRepository
    {
        Task Create(Domain.Entities.CommentReply commentReply);
        Task<Domain.Entities.CommentReply> ReadById(Guid id);
        Task<List<Domain.Entities.CommentReply>> ListByCommentId(Guid CommentId);
        Task Update(Domain.Entities.CommentReply commentReply);
        Task Delete(Guid id);
        Task DeleteAllByCommentId(Guid CommentId);
        Task<Domain.EntityRevisions.CommentReplyRevision> GetRevisions(Guid Id);
        
    }
}