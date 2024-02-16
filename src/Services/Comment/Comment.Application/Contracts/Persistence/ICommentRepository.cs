

namespace Comment.Application.Contracts.Persistence
{
    public interface ICommentRepository
    {
        Task CreateComment(Domain.Entities.Comment comment);
        Task<Domain.Entities.Comment> ReadCommentById(Guid id);

        Task<List<Domain.Entities.Comment>> GetCommentList();
        Task<List<Domain.Entities.Comment>> GetCommentListByResourceId(Guid resourceId);

        Task UpdateComment(Domain.Entities.Comment comment);
        Task DeleteComment(Guid id);
        Task<Domain.EntityRevisions.CommentRevision> GetCommentRevisions(Guid Id);
        
    }
}