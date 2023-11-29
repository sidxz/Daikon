using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using CQRS.Core.Domain;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.EntityRevisions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Gene.Infrastructure.Query.Repositories
{
    public class GeneRepository : IGeneRepository
    {

        private readonly IMongoCollection<Domain.Entities.Gene> _geneCollection;
        private readonly IVersionHub<GeneRevision> _versionHub;
        private readonly ILogger<GeneRepository> _logger;

        public GeneRepository(IConfiguration configuration, IVersionHub<GeneRevision> versionMaintainer, ILogger<GeneRepository> logger)
        {
            var client = new MongoClient(configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName"));
            _geneCollection = database.GetCollection<Domain.Entities.Gene>(configuration.GetValue<string>("GeneMongoDbSettings:GeneCollectionName"));
            _geneCollection.Indexes.CreateOne(new CreateIndexModel<Domain.Entities.Gene>(Builders<Domain.Entities.Gene>.IndexKeys.Ascending(g => g.AccessionNumber), new CreateIndexOptions { Unique = true }));

            _versionHub = versionMaintainer ?? throw new ArgumentNullException(nameof(versionMaintainer));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateGene(Domain.Entities.Gene gene)
        {

            ArgumentNullException.ThrowIfNull(gene);

            try
            {
                _logger.LogInformation("CreateGene: Creating gene {GeneId}, {Gene}", gene.Id, gene.ToJson());
                await _geneCollection.InsertOneAsync(gene);
                await _versionHub.CommitVersion(gene);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the gene with ID {GeneId}", gene.Id);
                throw new RepositoryException(nameof(GeneRepository), "Error creating gene", ex);
            }
        }



        public Task<Domain.Entities.Gene> ReadGeneById(Guid id)
        {
            return _geneCollection.Find(gene => gene.Id == id).FirstOrDefaultAsync();
        }

        public Task<Domain.Entities.Gene> ReadGeneByAccession(string accessionNumber)
        {
            return _geneCollection.Find(gene => gene.AccessionNumber == accessionNumber).FirstOrDefaultAsync();
        }


        public async Task<List<Domain.Entities.Gene>> GetGenesList()
        {
            try
            {
                return await _geneCollection.Find(gene => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the gene list");
                throw new RepositoryException(nameof(GeneRepository), "Error getting gene list", ex);
            }

        }

        public async Task UpdateGene(Domain.Entities.Gene gene)
        {
            ArgumentNullException.ThrowIfNull(gene);

            var filter = Builders<Domain.Entities.Gene>.Filter.Eq(g => g.Id, gene.Id);
            var update = Builders<Domain.Entities.Gene>.Update
                .Set(g => g.Name, gene.Name)
                .Set(g => g.Function, gene.Function)
                .Set(g => g.Product, gene.Product)
                .Set(g => g.FunctionalCategory, gene.FunctionalCategory);

            try
            {
                _logger.LogInformation("UpdateGene: Creating gene {GeneId}, {Gene}", gene.Id, gene.ToJson());
                await _geneCollection.UpdateOneAsync(filter, update);
                await _versionHub.CommitVersion(gene);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the gene with ID {GeneId}", gene.Id);
                throw new RepositoryException(nameof(GeneRepository), "Error updating gene", ex);
            }

        }


        public async Task DeleteGene(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);

            try
            {
                _logger.LogInformation("DeleteGene: Deleting gene {GeneId}", id);
                await _geneCollection.DeleteOneAsync(gene => gene.Id == id);
                await _versionHub.ArchiveEntity(id);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the gene with ID {GeneId}", id);
                throw new RepositoryException(nameof(GeneRepository), "Error deleting gene", ex);
            }

        }



        public async Task<GeneRevision> GetGeneRevisions(Guid Id)
        {
            var geneRevision = await _versionHub.GetVersions(Id);
            return geneRevision;
        }
    }
}