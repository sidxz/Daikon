
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Entities;
using MongoDB.Driver;

namespace MLogix.Infrastructure.Query.Repositories
{
    public class MoleculePredictionsRepository : IMoleculePredictionRepository
    {
        private readonly IMongoCollection<MoleculePredictions> _moleculePredictionsCollection;
        private readonly ILogger<MoleculePredictionsRepository> _logger;

        public MoleculePredictionsRepository(IConfiguration configuration, ILogger<MoleculePredictionsRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("MLxMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("MLxMongoDbSettings:DatabaseName"));
            _moleculePredictionsCollection = database.GetCollection<MoleculePredictions>(configuration.GetValue<string>("MLxMongoDbSettings:MoleculePredictionsCollectionName"));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MoleculePredictions> GetByMoleculeIdAsync(Guid moleculeId)
        {
            var filter = Builders<MoleculePredictions>.Filter.Eq(x => x.MoleculeId, moleculeId);
            return await _moleculePredictionsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<MoleculePredictions>> GetByMoleculeIdsAsync(List<Guid> moleculeIds)
        {
            if (moleculeIds == null || moleculeIds.Count == 0)
                return [];

            var filter = Builders<MoleculePredictions>.Filter.In(x => x.MoleculeId, moleculeIds);
            return await _moleculePredictionsCollection.Find(filter).ToListAsync();
        }

        public async Task UpsertAsync(MoleculePredictions prediction)
        {
            ArgumentNullException.ThrowIfNull(prediction);

            var filter = Builders<MoleculePredictions>.Filter.Eq(x => x.MoleculeId, prediction.MoleculeId);
            await _moleculePredictionsCollection.ReplaceOneAsync(
                filter,
                prediction,
                new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertBatchAsync(List<MoleculePredictions> predictions)
        {
            if (predictions == null || predictions.Count == 0)
                return;

            var models = predictions.Select(prediction =>
                new ReplaceOneModel<MoleculePredictions>(
                    Builders<MoleculePredictions>.Filter.Eq(x => x.MoleculeId, prediction.MoleculeId),
                    prediction
                )
                { IsUpsert = true }
            ).ToList();

            await _moleculePredictionsCollection.BulkWriteAsync(models, new BulkWriteOptions { IsOrdered = false });
        }
    }
}
