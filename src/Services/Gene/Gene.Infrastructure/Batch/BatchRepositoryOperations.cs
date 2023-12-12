
using Gene.Application.Contracts.Infrastructure;
using Gene.Domain.Batch;

using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Gene.Infrastructure.Batch
{
    public class BatchRepositoryOperations : IBatchRepositoryOperations
    {
        private readonly IMongoCollection<Domain.Entities.Gene> _geneCollection;
        private readonly IMongoCollection<Domain.Entities.Essentiality> _essentialityCollection;

        public BatchRepositoryOperations(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));

            _geneCollection = database.GetCollection<Domain.Entities.Gene>(configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName"));
            _essentialityCollection = database.GetCollection<Domain.Entities.Essentiality>(
                configuration.GetValue<string>("GeneMongoDbSettings:GeneEssentialityCollectionName") ??
                configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName") + "Essentiality");
        }

        // Get a dump of all genes and their essentialities from both collections
        public async Task<List<GeneExport>> GetAll()
        {
            var geneExport = await _geneCollection.Aggregate()
                .Lookup<Domain.Entities.Gene, Domain.Entities.Essentiality, GeneExport>(
                    _essentialityCollection,
                    gene => gene.Id,
                    essentiality => essentiality.GeneId,
                    geneExport => geneExport.Essentialities
                ).ToListAsync();

            return geneExport;
        }

    }
}