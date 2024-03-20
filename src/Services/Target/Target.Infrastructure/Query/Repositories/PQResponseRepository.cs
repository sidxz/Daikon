
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Target.Application.Contracts.Persistence;

namespace Target.Infrastructure.Query.Repositories
{
    public class PQResponseRepository : IPQResponseRepository
    {
        private readonly IMongoCollection<Domain.Entities.PQResponse> _pqResponseCollection;
        private readonly ILogger<PQResponseRepository> _logger;

        public PQResponseRepository(IConfiguration configuration, ILogger<PQResponseRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("TargetMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("TargetMongoDbSettings:DatabaseName"));
            _pqResponseCollection = database.GetCollection<Domain.Entities.PQResponse>(configuration.GetValue<string>("TargetMongoDbSettings:PromotionResponseCollectionName"));
            _pqResponseCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.PQResponse>(Builders<Domain.Entities.PQResponse>.IndexKeys.Ascending(t => t.TargetId), new CreateIndexOptions { Unique = false }));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Domain.Entities.PQResponse> Create(Domain.Entities.PQResponse pqResponse)
        {
            ArgumentNullException.ThrowIfNull(pqResponse);
            try
            {
                await _pqResponseCollection.InsertOneAsync(pqResponse);
                return pqResponse;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in CreatePQResponse");
                throw;
            }
        }

        //list all PQResponses
        public async Task<List<Domain.Entities.PQResponse>> ReadAll()
        {
            try
            {
                return await _pqResponseCollection.Find(p => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadAllPQResponses");
                throw;
            }
        }

        // list all unapproved PQResponses IsApproved = false
        public async Task<List<Domain.Entities.PQResponse>> ReadPendingVerification()
        {
            try
            {
                return await _pqResponseCollection.Find(p => p.IsVerified == false).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadAllDraftPQResponses");
                throw;
            }
        }

        // list all approved PQResponses IsApproved = true
        public async Task<List<Domain.Entities.PQResponse>> ReadApproved()
        {
            try
            {
                return await _pqResponseCollection.Find(p => p.IsVerified == true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadAllApprovedPQResponses");
                throw;
            }
        }

        public async Task<Domain.Entities.PQResponse> ReadById(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                return await _pqResponseCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadPQResponseById");
                throw;
            }
        }

        public async Task<Domain.Entities.PQResponse> ReadByTargetId(Guid targetId)
        {
            ArgumentNullException.ThrowIfNull(targetId);
            try
            {
                return await _pqResponseCollection.Find(p => p.TargetId == targetId).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in ReadPQResponseByTargetId");
                throw;
            }
        }

        public async Task<Domain.Entities.PQResponse> Update(Domain.Entities.PQResponse pqResponse)
        {
            ArgumentNullException.ThrowIfNull(pqResponse);
            try
            {
                await _pqResponseCollection.ReplaceOneAsync(p => p.Id == pqResponse.Id, pqResponse);
                return pqResponse;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in UpdatePQResponse");
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            try
            {
                await _pqResponseCollection.DeleteOneAsync(p => p.Id == id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error in DeletePQResponse");
                throw;
            }
        }

    }
}