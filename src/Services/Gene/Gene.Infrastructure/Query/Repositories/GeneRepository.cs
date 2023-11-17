using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gene.Application.Contracts.Persistence;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Gene.Infrastructure.Query.Repositories
{
    public class GeneRepository : IGeneRepository
    {

        private readonly IMongoCollection<Domain.Entities.Gene> _geneCollection;

        public GeneRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _geneCollection = database.GetCollection<Domain.Entities.Gene>(configuration.GetValue<string>("GeneMongoDbSettings:CollectionName"));
        }

        public async Task CreateGene(Domain.Entities.Gene gene)
        {
            await _geneCollection.InsertOneAsync(@gene);
        }

        public Task DeleteGene(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task ReadGene(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Domain.Entities.Gene>> GetGenesList()
        {
            return await _geneCollection.Find(gene => true).ToListAsync();
        }

        public Task UpdateGene(Domain.Entities.Gene gene)
        {
            throw new NotImplementedException();
        }
    }
}