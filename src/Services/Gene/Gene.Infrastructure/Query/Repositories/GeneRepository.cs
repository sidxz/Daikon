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

        public Task<Domain.Entities.Gene> ReadGeneById(Guid id)
        {
            return _geneCollection.Find(gene => gene.Id == id).FirstOrDefaultAsync();
        }

        public Task<Domain.Entities.Gene> ReadGeneByName(string name)
        {
            return _geneCollection.Find(gene => gene.Name == name).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.Gene>> GetGenesList()
        {
            return await _geneCollection.Find(gene => true).ToListAsync();
        }

        public async Task UpdateGene(Domain.Entities.Gene gene)
        {
            Console.WriteLine("++++++++++ GeneRepository.UpdateGene");
            Console.WriteLine("gene.Id: " + gene.Id);
            if (gene == null)
            {
                throw new ArgumentNullException(nameof(gene));
            }

            var filter = Builders<Domain.Entities.Gene>.Filter.Eq(g => g.Id, gene.Id);
            var update = Builders<Domain.Entities.Gene>.Update
                .Set(g => g.Name, gene.Name)
                .Set(g => g.Function, gene.Function)
                .Set(g => g.Product, gene.Product)
                .Set(g => g.FunctionalCategory, gene.FunctionalCategory);

            try
            {
                Console.WriteLine("await _geneCollection.UpdateOneAsync");
                await _geneCollection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine("----- EXCEPTION  GeneRepository.UpdateGene: " + ex.Message);
                // Handle or log the exception as appropriate for your application
                throw;
            }
            
        }

    }
}