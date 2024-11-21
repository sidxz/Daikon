using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Domain.Entities;
using DocuStore.Domain.EntityRevisions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;


namespace DocuStore.Infrastructure.Repositories
{
    public class ParsedDocRepository : IParsedDocRepository
    {
        private readonly IMongoCollection<ParsedDoc> _parsedDocCollection;
        private readonly ILogger<ParsedDocRepository> _logger;
        private readonly IVersionHub<ParsedDocRevision> _versionHub;

        public ParsedDocRepository(
            IConfiguration configuration,
            ILogger<ParsedDocRepository> logger,
            IVersionHub<ParsedDocRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("ParsedDocMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("ParsedDocMongoDbSettings:DatabaseName"));
            _parsedDocCollection = database.GetCollection<ParsedDoc>(configuration.GetValue<string>("ParsedDocMongoDbSettings:CollectionName"));

            // Create indexes for optimized queries
            CreateIndexes();

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void CreateIndexes()
        {
            _parsedDocCollection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<ParsedDoc>(Builders<ParsedDoc>.IndexKeys.Ascending(doc => doc.Tags), new CreateIndexOptions { Unique = false }),
                new CreateIndexModel<ParsedDoc>(Builders<ParsedDoc>.IndexKeys.Ascending(doc => doc.PublicationDate), new CreateIndexOptions { Unique = false }),
                new CreateIndexModel<ParsedDoc>(Builders<ParsedDoc>.IndexKeys.Ascending(doc => doc.Mentions), new CreateIndexOptions { Unique = false }),
                new CreateIndexModel<ParsedDoc>(Builders<ParsedDoc>.IndexKeys.Ascending(doc => doc.PublicationDate), new CreateIndexOptions { Unique = false }),
                new CreateIndexModel<ParsedDoc>(Builders<ParsedDoc>.IndexKeys.Hashed(doc => doc.DocHash), new CreateIndexOptions { Unique = true }) // Ensure unique hash
            });
        }

        public async Task Create(ParsedDoc parsedDoc)
        {
            ArgumentNullException.ThrowIfNull(parsedDoc);

            try
            {
                _logger.LogInformation("CreateParsedDoc: Creating parsed document {ParsedDocId}, {ParsedDoc}", parsedDoc.Id, parsedDoc.ToJson());
                await _parsedDocCollection.InsertOneAsync(parsedDoc);
                await _versionHub.CommitVersion(parsedDoc);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogError(ex, "Duplicate document hash detected for {DocHash}", parsedDoc.DocHash);
                throw new RepositoryException(nameof(ParsedDocRepository), "Duplicate document hash detected", ex);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the parsed document with ID {ParsedDocId}", parsedDoc.Id);
                throw new RepositoryException(nameof(ParsedDocRepository), "Error creating parsed document", ex);
            }
        }

        public async Task<ParsedDoc> ReadById(Guid id)
        {
            try
            {
                var parsedDoc = await _parsedDocCollection.Find(doc => doc.Id == id).FirstOrDefaultAsync();
                if (parsedDoc == null)
                {
                    throw new ResourceNotFoundException(nameof(ParsedDoc), id);
                }
                return parsedDoc;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the parsed document with ID {ParsedDocId}", id);
                throw new RepositoryException(nameof(ParsedDocRepository), "Error reading parsed document", ex);
            }
        }

        public async Task<ParsedDoc> ReadByPath(string filePath)
        {
            try
            {
                return await _parsedDocCollection.Find(doc => doc.FilePath == filePath).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the parsed document by file path {FilePath}", filePath);
                throw new RepositoryException(nameof(ParsedDocRepository), "Error reading parsed document by file path", ex);
            }
        }

        public async Task<ParsedDoc> ReadByHash(string docHash)
        {
            try
            {
                if (string.IsNullOrEmpty(docHash))
                {
                    throw new ArgumentException("Document hash cannot be null or empty.", nameof(docHash));
                }

                // Query the collection for the specified hash
                return await _parsedDocCollection.Find(doc => doc.DocHash == docHash).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the parsed document by hash {DocHash}", docHash);
                throw new RepositoryException(nameof(ParsedDocRepository), "Error reading parsed document by hash", ex);
            }
        }

        public async Task<List<ParsedDoc>> ListAll(
                                                    int limit = 1000,
                                                    DateTime? startDate = null,
                                                    DateTime? endDate = null)
        {
            try
            {
                var filters = new List<FilterDefinition<ParsedDoc>>();

                // Apply date filters if provided
                if (startDate.HasValue)
                {
                    filters.Add(Builders<ParsedDoc>.Filter.Gte(doc => doc.PublicationDate.Value, startDate.Value));
                }
                if (endDate.HasValue)
                {
                    filters.Add(Builders<ParsedDoc>.Filter.Lte(doc => doc.PublicationDate.Value, endDate.Value));
                }

                // Combine filters into a single filter (if any exist)
                var combinedFilter = filters.Count > 0
                    ? Builders<ParsedDoc>.Filter.And(filters)
                    : Builders<ParsedDoc>.Filter.Empty;

                // Query the database with filters and limit
                return await _parsedDocCollection.Find(combinedFilter).Limit(limit).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while listing parsed documents with filters");
                throw new RepositoryException(nameof(ParsedDocRepository), "Error listing parsed documents", ex);
            }
        }


        public async Task<List<ParsedDoc>> ListMostRecent(int count)
        {
            try
            {
                return await _parsedDocCollection.Find(_ => true).SortByDescending(doc => doc.PublicationDate).Limit(count).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while listing the most recent parsed documents");
                throw new RepositoryException(nameof(ParsedDocRepository), "Error listing most recent parsed documents", ex);
            }
        }

        public async Task<List<ParsedDoc>> ListByTags(
                                                        List<string> tags,
                                                        int limit = 1000,
                                                        DateTime? startDate = null,
                                                        DateTime? endDate = null
                                                        )
        {
            try
            {
                if (tags == null || !tags.Any())
                {
                    throw new ArgumentException("Tags list cannot be null or empty.", nameof(tags));
                }

                // Build the filter criteria
                var filters = new List<FilterDefinition<ParsedDoc>>
        {
            Builders<ParsedDoc>.Filter.Or(
                tags.Select(tag => Builders<ParsedDoc>.Filter.Regex("Tags", new BsonRegularExpression(tag, "i")))),
            Builders<ParsedDoc>.Filter.Gte(doc => doc.PublicationDate.Value, startDate ?? DateTime.MinValue),
            Builders<ParsedDoc>.Filter.Lte(doc => doc.PublicationDate.Value, endDate ?? DateTime.UtcNow)
        };

                var combinedFilter = Builders<ParsedDoc>.Filter.And(filters);

                // Execute the query with a limit on the number of results
                return await _parsedDocCollection.Find(combinedFilter).Limit(limit).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while listing parsed documents by tags");
                throw new RepositoryException(nameof(ParsedDocRepository), "Error listing parsed documents by tags", ex);
            }
        }


        public async Task<List<ParsedDoc>> ListByMentions(List<Guid> mentions)
        {
            try
            {
                return await _parsedDocCollection.Find(doc => doc.Mentions.Any(mention => mentions.Contains(mention))).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while listing parsed documents by mentions");
                throw new RepositoryException(nameof(ParsedDocRepository), "Error listing parsed documents by mentions", ex);
            }
        }

        public async Task Update(ParsedDoc parsedDoc)
        {
            ArgumentNullException.ThrowIfNull(parsedDoc);

            try
            {
                _logger.LogInformation("UpdateParsedDoc: Updating parsed document {ParsedDocId}, {ParsedDoc}", parsedDoc.Id, parsedDoc.ToJson());
                await _parsedDocCollection.ReplaceOneAsync(doc => doc.Id == parsedDoc.Id, parsedDoc);
                await _versionHub.CommitVersion(parsedDoc);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the parsed document with ID {ParsedDocId}", parsedDoc.Id);
                throw new RepositoryException(nameof(ParsedDocRepository), "Error updating parsed document", ex);
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("DeleteParsedDoc: Deleting parsed document {ParsedDocId}", id);
                await _parsedDocCollection.DeleteOneAsync(doc => doc.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the parsed document with ID {ParsedDocId}", id);
                throw new RepositoryException(nameof(ParsedDocRepository), "Error deleting parsed document", ex);
            }
        }

        public async Task<ParsedDocRevision> GetRevisions(Guid id)
        {
            return await _versionHub.GetVersions(id);
        }
    }
}
