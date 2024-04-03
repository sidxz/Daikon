
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
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    // Create the index if it does not exist
                    var createIndexQuery = "CREATE INDEX gene_accessionNo_index IF NOT EXISTS FOR (g:Gene) ON (g.accessionNumber);";
                    await tx.RunAsync(createIndexQuery);
                    createIndexQuery = "CREATE INDEX strain_name_index IF NOT EXISTS FOR (s:Strain) ON (s.name);";

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateConstraintsAsync()
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {

                    // functional category constraint; unique
                    var createConstraintQuery = "CREATE CONSTRAINT uniId_constraint IF NOT EXISTS FOR (g:Gene) REQUIRE g.uniId IS UNIQUE;";
                    await tx.RunAsync(createConstraintQuery);

                });
            }
            finally
            {
                await session.CloseAsync();
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
                                        s.strainId = $strainId,
                                        s.organism = $organism,
                            ON MATCH SET  
                                        s.name = $name,
                                        s.strainId = $strainId,
                                        s.organism = $organism,
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = strain.UniId,
                                 name = strain.Name,
                                 strainId = strain.StrainId,
                                 organism = strain.Organism
                             }).ExecuteAsync()
                             ;
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
                            SET s.name = $name, s.organism = $organism
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = strain.UniId,
                                 name = strain.Name,
                                 strainId = strain.StrainId,
                                 organism = strain.Organism
                             }).ExecuteAsync()
                             ;
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
