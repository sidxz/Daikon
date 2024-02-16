
namespace Comment.Application.Contracts.Persistence
{
    public interface ICommentReplyRepository
    {
        Task CreateCommentReply(Domain.Entities.CommentReply commentReply);
        Task<Domain.Entities.CommentReply> ReadCommentReplyById(Guid id);
        Task<List<Domain.Entities.CommentReply>> GetCommentReplyOfComment(Guid CommentId);
        Task UpdateCommentReply(Domain.Entities.CommentReply commentReply);
        Task DeleteCommentReply(Guid id);
        Task<Domain.EntityRevisions.CommentReplyRevision> GetCommentReplyRevisions(Guid Id);
        
    }
}