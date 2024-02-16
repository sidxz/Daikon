
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.EntityRevisions;

namespace Comment.Infrastructure.Query.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Domain.Entities.Comment> _commentCollection;
        private readonly ILogger<CommentRepository> _logger;
        private readonly IVersionHub<CommentRevision> _versionHub;

        public CommentRepository(IConfiguration configuration, ILogger<CommentRepository> logger, IVersionHub<CommentRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("CommentMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("CommentMongoDbSettings:DatabaseName"));
            _commentCollection = database.GetCollection<Domain.Entities.Comment>(configuration.GetValue<string>("CommentMongoDbSettings:CommentCollectionName"));
            _commentCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Comment>(Builders<Domain.Entities.Comment>.IndexKeys.Ascending(t => t.ResourceId), new CreateIndexOptions { Unique = false }));
            _commentCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Comment>(Builders<Domain.Entities.Comment>.IndexKeys.Ascending(t => t.Topic), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateComment(Domain.Entities.Comment comment)
        {

            ArgumentNullException.ThrowIfNull(comment);

            try
            {
                _logger.LogInformation("CreateComment: Creating comment {CommentId}, {Comment}", comment.Id, comment.ToJson());
                await _commentCollection.InsertOneAsync(comment);
                await _versionHub.CommitVersion(comment);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the comment with ID {CommentId}", comment.Id);
                throw new RepositoryException(nameof(CommentRepository), "Error creating comment", ex);
            }
        }

        public async Task<Domain.Entities.Comment> ReadCommentById(Guid id)
        {
            return await _commentCollection.Find(comment => comment.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Domain.Entities.Comment>> GetCommentList()
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

        public async Task<List<Domain.Entities.Comment>> GetCommentListByResourceId(Guid resourceId)
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

        public async Task UpdateComment(Domain.Entities.Comment comment)
        {
            ArgumentNullException.ThrowIfNull(comment);

            try
            {
                _logger.LogInformation("UpdateComment: Updating comment {CommentId}, {Comment}", comment.Id, comment.ToJson());
                await _commentCollection.ReplaceOneAsync(c => c.Id == comment.Id, comment);
                await _versionHub.CommitVersion(comment);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the comment with ID {CommentId}", comment.Id);
                throw new RepositoryException(nameof(CommentRepository), "Error updating comment", ex);
            }
        }

        public async Task DeleteComment(Guid id)
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

        public async Task<CommentRevision> GetCommentRevisions(Guid Id)
        {
            return await _versionHub.GetVersions(Id);
        }
    }
        
}