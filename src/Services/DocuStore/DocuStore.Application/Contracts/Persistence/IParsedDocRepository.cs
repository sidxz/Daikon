
using DocuStore.Domain.Entities;
using DocuStore.Domain.EntityRevisions;

namespace DocuStore.Application.Contracts.Persistence
{
    /// <summary>
    /// Repository interface for managing ParsedDoc entities.
    /// Provides methods for CRUD operations and specialized queries.
    /// </summary>
    public interface IParsedDocRepository
    {
        /// <summary>
        /// Creates a new ParsedDoc entity in the repository.
        /// </summary>
        /// <param name="parsedDoc">The ParsedDoc entity to create.</param>
        Task Create(ParsedDoc parsedDoc);

        /// <summary>
        /// Retrieves a ParsedDoc entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the ParsedDoc.</param>
        /// <returns>The ParsedDoc entity, or null if not found.</returns>
        Task<ParsedDoc> ReadById(Guid id);

        /// <summary>
        /// Retrieves a ParsedDoc entity by its file path.
        /// </summary>
        /// <param name="filePath">The file path of the ParsedDoc.</param>
        /// <returns>The ParsedDoc entity, or null if not found.</returns>
        Task<ParsedDoc> ReadByPath(string filePath);


        /// <summary>
        /// Retrieves a ParsedDoc entity by its hash.
        /// </summary>
        /// <param name="docHash">The hash of the ParsedDoc.</param>
        /// <returns>The ParsedDoc entity, or null if not found.</returns>
         Task<ParsedDoc> ReadByHash(string docHash);

        /// <summary>
        /// Retrieves all ParsedDoc entities in the repository.
        /// Consider using this method cautiously for large datasets.
        /// <param name="limit">The maximum number of results to return. Defaults to 1000 if not specified.</param>
        /// <param name="startDate">The start date for filtering documents. Defaults to null, which implies no lower bound.</param>
        /// <param name="endDate">The end date for filtering documents. Defaults to today's date if not specified.</param>
        /// </summary>
        /// <returns>A list of all ParsedDoc entities.</returns>
        Task<List<ParsedDoc>> ListAll(int limit = 1000,
                                                    DateTime? startDate = null,
                                                    DateTime? endDate = null);

        /// <summary>
        /// Retrieves the most recent ParsedDoc entities.
        /// </summary>
        /// <param name="count">The number of most recent ParsedDocs to retrieve.</param>
        /// <returns>A list of the most recent ParsedDoc entities.</returns>
        Task<List<ParsedDoc>> ListMostRecent(int count);


        /// Retrieves ParsedDoc entities that match the specified tags, within an optional date range and a result limit.
        /// </summary>
        /// <param name="tags">The list of tags to filter by. Must not be null or empty.</param>
        /// <param name="limit">The maximum number of results to return. Defaults to 1000 if not specified.</param>
        /// <param name="startDate">The start date for filtering documents. Defaults to null, which implies no lower bound.</param>
        /// <param name="endDate">The end date for filtering documents. Defaults to today's date if not specified.</param>
        /// <returns>A list of ParsedDoc entities matching the specified criteria.</returns>
        Task<List<ParsedDoc>> ListByTags(
            List<string> tags,
            int limit = 1000,
            DateTime? startDate = null,
            DateTime? endDate = null);

        /// <summary>
        /// Retrieves ParsedDoc entities that mention the specified entities.
        /// </summary>
        /// <param name="mentions">The list of entity IDs to filter by.</param>
        /// <returns>A list of ParsedDoc entities mentioning the specified IDs.</returns>
        Task<List<ParsedDoc>> ListByMentions(List<Guid> mentions);

        /// <summary>
        /// Updates an existing ParsedDoc entity in the repository.
        /// </summary>
        /// <param name="parsedDoc">The ParsedDoc entity to update.</param>
        Task Update(ParsedDoc parsedDoc);

        /// <summary>
        /// Deletes a ParsedDoc entity from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the ParsedDoc to delete.</param>
        Task Delete(Guid id);

        /// <summary>
        /// Retrieves all revisions for a specified ParsedDoc entity.
        /// </summary>
        /// <param name="id">The unique identifier of the ParsedDoc.</param>
        /// <returns>A list of revisions for the specified ParsedDoc.</returns>
        Task<ParsedDocRevision> GetRevisions(Guid id);
    }
}
