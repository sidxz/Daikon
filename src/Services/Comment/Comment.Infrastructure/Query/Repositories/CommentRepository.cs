
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Comment.Application.Contracts.Persistence;
using MongoDB.Bson;

namespace Comment.Infrastructure.Query.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Domain.Entities.Comment> _commentCollection;
        private readonly ILogger<CommentRepository> _logger;
        public CommentRepository(IConfiguration configuration, ILogger<CommentRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("CommentMongoDbSettings:ConnectionString"));

            var database = client.GetDatabase(configuration.GetValue<string>("CommentMongoDbSettings:DatabaseName"));
            _commentCollection = database.GetCollection<Domain.Entities.Comment>(configuration.GetValue<string>("CommentMongoDbSettings:CommentCollectionName"));

            _commentCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Comment>
            (Builders<Domain.Entities.Comment>.IndexKeys.Ascending(t => t.ResourceId), new CreateIndexOptions { Unique = false }));

            _commentCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Comment>(
                        Builders<Domain.Entities.Comment>.IndexKeys.Ascending(t => t.Tags), new CreateIndexOptions { Unique = false }));

            _commentCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Comment>(
            Builders<Domain.Entities.Comment>.IndexKeys.Ascending(t => t.Mentions), new CreateIndexOptions { Unique = false }));

            _commentCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Comment>(
            Builders<Domain.Entities.Comment>.IndexKeys.Ascending(t => t.Subscribers), new CreateIndexOptions { Unique = false }));

            _commentCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Comment>(
            Builders<Domain.Entities.Comment>.IndexKeys.Ascending(t => t.DateCreated), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(Domain.Entities.Comment comment)
        {

            ArgumentNullException.ThrowIfNull(comment);

            try
            {
                _logger.LogInformation("CreateComment: Creating comment {CommentId}, {Comment}", comment.Id, comment.ToJson());
                await _commentCollection.InsertOneAsync(comment);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the comment with ID {CommentId}", comment.Id);
                throw new RepositoryException(nameof(CommentRepository), "Error creating comment", ex);
            }
        }

        public async Task<Domain.Entities.Comment> ReadById(Guid id)
        {
            try
            {
                return await _commentCollection.Find(comment => comment.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while reading the comment with ID {CommentId}", id);
                throw new RepositoryException(nameof(CommentRepository), "Error reading comment", ex);
            }
        }

        public async Task<List<Domain.Entities.Comment>> ListAll()
        {
            try
            {
                return await _commentCollection.Find(_ => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the comment list");
                throw new RepositoryException(nameof(CommentRepository), "Error getting comment list", ex);
            }
        }

        public async Task<List<Domain.Entities.Comment>> ListMostRecent(int count)
        {
            try
            {
                return await _commentCollection.Find(_ => true).SortByDescending(comment => comment.DateCreated).Limit(count).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the comment list");
                throw new RepositoryException(nameof(CommentRepository), "Error getting comment list", ex);
            }
        }

        public async Task<List<Domain.Entities.Comment>> ListByResourceId(Guid resourceId)
        {
            try
            {
                return await _commentCollection.Find(comment => comment.ResourceId == resourceId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the comment list");
                throw new RepositoryException(nameof(CommentRepository), "Error getting comment list", ex);
            }
        }

        public async Task<List<Domain.Entities.Comment>> ListByUserId(Guid userId)
        {
            try
            {
                return await _commentCollection.Find(comment => comment.CreatedById == userId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the comment list");
                throw new RepositoryException(nameof(CommentRepository), "Error getting comment list", ex);
            }
        }

        public async Task<List<Domain.Entities.Comment>> ListByTags(List<string> tags)
        {
            try
            {
                var regexFilters = tags.Select(tag => Builders<Domain.Entities.Comment>.Filter.Regex("Tags", new BsonRegularExpression(tag, "i"))).ToList();
                var combinedFilter = Builders<Domain.Entities.Comment>.Filter.Or(regexFilters);
                return await _commentCollection.Find(combinedFilter).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the comment list");
                throw new RepositoryException(nameof(CommentRepository), "Error getting comment list", ex);
            }
        }

        public async Task<List<Domain.Entities.Comment>> ListByMentions(List<Guid> mentions)
        {
            try
            {
                return await _commentCollection.Find(comment => comment.Mentions.Any(mention => mentions.Contains(mention))).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the comment list");
                throw new RepositoryException(nameof(CommentRepository), "Error getting comment list", ex);
            }
        }

        public async Task<List<Domain.Entities.Comment>> ListBySubscribers(List<Guid> subscribers)
        {
            try
            {
                return await _commentCollection.Find(comment => comment.Subscribers.Any(subscriber => subscribers.Contains(subscriber))).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the comment list");
                throw new RepositoryException(nameof(CommentRepository), "Error getting comment list", ex);
            }
        }


        public async Task Update(Domain.Entities.Comment comment)
        {
            ArgumentNullException.ThrowIfNull(comment);

            try
            {
                _logger.LogInformation("UpdateComment: Updating comment {CommentId}, {Comment}", comment.Id, comment.ToJson());
                await _commentCollection.ReplaceOneAsync(c => c.Id == comment.Id, comment);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the comment with ID {CommentId}", comment.Id);
                throw new RepositoryException(nameof(CommentRepository), "Error updating comment", ex);
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("DeleteComment: Deleting comment {CommentId}", id);
                await _commentCollection.DeleteOneAsync(c => c.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the comment with ID {CommentId}", id);
                throw new RepositoryException(nameof(CommentRepository), "Error deleting comment", ex);
            }
        }
    }
}