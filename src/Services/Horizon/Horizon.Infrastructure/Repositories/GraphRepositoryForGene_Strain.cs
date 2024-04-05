
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Genes;
using Horizon.Domain.Strains;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Polly;

namespace Horizon.Infrastructure.Repositories
{
    public partial class GraphRepositoryForGene : IGraphRepositoryForGene
    {
        private readonly IDriver _driver;
        private ILogger<GraphRepositoryForGene> _logger;

        public GraphRepositoryForGene(IDriver driver, ILogger<GraphRepositoryForGene> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task CreateIndexesAsync()
        {

            try
            {
                // var query = @"
                //   CREATE INDEX gene_uniId_index IF NOT EXISTS FOR (g:Gene) ON (g.uniId);
                // ";
                // var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();

                var query2 = @"
                  CREATE INDEX gene_accessionNo_index IF NOT EXISTS FOR (g:Gene) ON (g.accessionNumber);
                ";
                var (query2Results, _) = await _driver.ExecutableQuery(query2).ExecuteAsync();


                // var query3 = @"
                //   CREATE INDEX strain_uni_index IF NOT EXISTS FOR (s:Strain) ON (s.uniId);
                // ";
                // var (query3Results, _) = await _driver.ExecutableQuery(query3).ExecuteAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateIndexesAsync");
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Creating Indexes In Graph", ex);
            }

        }

        public async Task CreateConstraintsAsync()
        {
            try
            {
                var query1 = @"
                    CREATE CONSTRAINT gene_uniId_unique IF NOT EXISTS FOR (g:Gene) REQUIRE g.uniId IS UNIQUE;
                ";
                var (queryResults1, _) = await _driver.ExecutableQuery(query1).ExecuteAsync();

                var query2 = @"
                    CREATE CONSTRAINT strain_uniId_unique IF NOT EXISTS FOR (s:Strain) REQUIRE s.uniId IS UNIQUE;
                ";
                var (queryResults2, _) = await _driver.ExecutableQuery(query2).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateConstraintsAsync");
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Creating Constraints In Graph", ex);
            }
        }

        public async Task AddStrain(Strain strain)
        {
            _logger.LogInformation("AddStrain(): Adding strain with name {Name} and id {StrainId}", strain.Name, strain.StrainId);

            try
            {
                var query = @"
                   MERGE (s:Strain { uniId: $uniId })
                            ON CREATE SET
                                        s.name = $name,
                                        s.organism = $organism
                            ON MATCH SET  
                                        s.name = $name,
                                        s.organism = $organism
                ";

                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = strain.StrainId,
                        name = strain.Name,
                        organism = strain.Organism
                    }).ExecuteAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddStrain with Name {Name}", strain.Name);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Adding Strain To Graph", ex);
            }


        }

        public async Task UpdateStrain(Strain strain)
        {
            _logger.LogInformation("UpdateStrain(): Updating strain with name {Name} and id {StrainId}", strain.Name, strain.StrainId);

            try
            {
                var query = @"
                     MATCH (s:Strain {uniId: $uniId})
                        SET
                            s.name = $name,
                            s.organism = $organism
                ";

                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = strain.StrainId,
                        name = strain.Name,
                        organism = strain.Organism
                    }).ExecuteAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStrain");
                _logger.LogError(ex, "All retry attempts failed for updating strain with Name {Name}", strain.Name);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Updating Strain Of Graph", ex);
            }
        }
    }
}
