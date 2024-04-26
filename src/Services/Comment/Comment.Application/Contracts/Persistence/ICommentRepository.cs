

namespace Comment.Application.Contracts.Persistence
{
    public interface ICommentRepository
    {
        Task Create(Domain.Entities.Comment comment);
        Task<Domain.Entities.Comment> ReadById(Guid id);
        Task<List<Domain.Entities.Comment>> ListAll();
        Task<List<Domain.Entities.Comment>> ListByResourceId(Guid resourceId);
        Task<List<Domain.Entities.Comment>> ListByUserId(Guid userId);
        Task<List<Domain.Entities.Comment>> ListByTags(List<string> tags);
        Task<List<Domain.Entities.Comment>> ListByMentions(List<Guid> mentions);
        Task<List<Domain.Entities.Comment>> ListBySubscribers(List<Guid> subscribers);

        Task Update(Domain.Entities.Comment comment);
        Task Delete(Guid id);
        Task<Domain.EntityRevisions.CommentRevision> GetRevisions(Guid Id);
        
    }
}