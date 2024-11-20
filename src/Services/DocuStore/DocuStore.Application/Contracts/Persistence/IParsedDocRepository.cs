

namespace DocuStore.Application.Contracts.Persistence
{
    public interface IParsedDocRepository
    {
        Task Create(Domain.Entities.ParsedDoc parsedDoc);
        Task<Domain.Entities.ParsedDoc> ReadById(Guid id);
        Task<Domain.Entities.ParsedDoc> ReadByPath(string filePath);
        Task<List<Domain.Entities.ParsedDoc>> ListAll();
        Task<List<Domain.Entities.ParsedDoc>> ListMostRecent(int count);

        Task<List<Domain.Entities.ParsedDoc>> ListByTags(List<string> tags);
        Task<List<Domain.Entities.ParsedDoc>> ListByMentions(List<Guid> mentions);

        Task Update(Domain.Entities.ParsedDoc parsedDoc);
        Task Delete(Guid id);
        Task<Domain.EntityRevisions.ParsedDocRevision> GetRevisions(Guid Id);

    }
}