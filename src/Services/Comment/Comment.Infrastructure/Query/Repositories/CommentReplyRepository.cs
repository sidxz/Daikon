
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Entities;
using Comment.Domain.EntityRevisions;

namespace Comment.Infrastructure.Query.Repositories
{
    public class CommentReplyRepository : ICommentReplyRepository
    {
        private readonly IMongoCollection<CommentReply> _commentReplyCollection;
        private readonly ILogger<CommentReplyRepository> _logger;
        private readonly IVersionHub<CommentReplyRevision> _versionHub;

        public CommentReplyRepository(IConfiguration configuration, ILogger<CommentReplyRepository> logger, IVersionHub<CommentReplyRevision> versionMaintainer)
        {
            var client = new MongoClient(configuration.GetValue<string>("CommentMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("CommentMongoDbSettings:DatabaseName"));
            _commentReplyCollection = database.GetCollection<CommentReply>(configuration.GetValue<string>("CommentMongoDbSettings:CommentReplyCollectionName"));
            _commentReplyCollection.Indexes.CreateOne(new CreateIndexModel<CommentReply>(Builders<CommentReply>.IndexKeys.Ascending(t => t.CommentId), new CreateIndexOptions { Unique = false }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateCommentReply(CommentReply commentReply)
        {

            ArgumentNullException.ThrowIfNull(commentReply);

            try
            {
                _logger.LogInformation("CreateCommentReply: Creating commentReply {CommentReplyId}, {CommentReply}", commentReply.Id, commentReply.ToJson());
                await _commentReplyCollection.InsertOneAsync(commentReply);
                await _versionHub.CommitVersion(commentReply);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the commentReply with ID {CommentReplyId}", commentReply.Id);
                throw new RepositoryException(nameof(CommentReplyRepository), "Error creating commentReply", ex);
            }
        }

        public async Task<CommentReply> ReadCommentReplyById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("ReadCommentReplyById: Getting commentReply {CommentReplyId}", id);
                return await _commentReplyCollection.Find(commentReply => commentReply.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the commentReply with ID {CommentReplyId}", id);
                throw new RepositoryException(nameof(CommentReplyRepository), "Error getting commentReply", ex);
            }
        }

        public async Task DeleteCommentReply(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                _logger.LogInformation("DeleteCommentReply: Deleting commentReply {CommentReplyId}", id);
                await _commentReplyCollection.DeleteOneAsync(commentReply => commentReply.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the commentReply with ID {CommentReplyId}", id);
                throw new RepositoryException(nameof(CommentReplyRepository), "Error deleting commentReply", ex);
            }
        }

        public async Task<List<CommentReply>> GetCommentReplyOfComment(Guid CommentId)
        {
            ArgumentNullException.ThrowIfNull(CommentId);
            try
            {
                _logger.LogInformation("GetCommentReplyOfComment: Getting commentReply list of comment {CommentId}", CommentId);
                return await _commentReplyCollection.Find(commentReply => commentReply.CommentId == CommentId).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the commentReply of comment with ID {CommentId}", CommentId);
                throw new RepositoryException(nameof(CommentReplyRepository), "Error getting commentReply list", ex);
            }
        }

       public async Task UpdateCommentReply(CommentReply commentReply)
       {
        ArgumentNullException.ThrowIfNull(commentReply);
        try
            {
                _logger.LogInformation("UpdateCommentReply: Updating commentReply {CommentReplyId}, {CommentReply}", commentReply.Id, commentReply.ToJson());
                await _commentReplyCollection.ReplaceOneAsync(t => t.Id == commentReply.Id, commentReply);
                await _versionHub.CommitVersion(commentReply);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the commentReply with ID {CommentReplyId}", commentReply.Id);
                throw new RepositoryException(nameof(CommentReplyRepository), "Error updating commentReply", ex);
            }
       }

       public async Task<CommentReplyRevision> GetCommentReplyRevisions(Guid Id)
        {
            var commentReplyRevision = await _versionHub.GetVersions(Id);
            return commentReplyRevision;
        }
    }
}