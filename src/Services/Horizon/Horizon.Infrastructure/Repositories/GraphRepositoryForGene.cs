
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Genes;
using Horizon.Domain.Strains;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Polly;

namespace Horizon.Infrastructure.Repositories
{
    public class GraphRepositoryForGene : IGraphRepositoryForGene
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
                    var createConstraintQuery = "CREATE CONSTRAINT functional_category_name_constraint IF NOT EXISTS FOR (fc:FunctionalCategory) REQUIRE fc.name IS UNIQUE;";
                    await tx.RunAsync(createConstraintQuery);

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task AddStrainToGraph(Strain strain)
        {
            _logger.LogInformation("AddStrainToGraph(): Adding strain with name {Name} and id {StrainId}", strain.Name, strain.StrainId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var createStrainQuery = @"
                            CREATE (s:Strain {name: $name, strainId: $strainId, organism: $organism})
                        ";


                        _logger.LogInformation("tx.RunAsync Adding strain with name {Name}", strain.Name);
                        await tx.RunAsync(createStrainQuery, new
                        {
                            name = strain.Name,
                            strainId = strain.StrainId,
                            organism = strain.Organism
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddStrainToGraph");
                _logger.LogError(ex, "All retry attempts failed for adding strain with Name {Name}", strain.Name);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Adding Strain To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateStrainOfGraph(Strain strain)
        {
            _logger.LogInformation("UpdateStrainOfGraph(): Updating strain with name {Name} and id {StrainId}", strain.Name, strain.StrainId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var updateStrainQuery = @"
                            MATCH (s:Strain {strainId: $strainId})
                            SET s.name = $name, s.organism = $organism
                        ";

                        await tx.RunAsync(updateStrainQuery, new
                        {
                            name = strain.Name,
                            strainId = strain.StrainId,
                            organism = strain.Organism
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStrainOfGraph");
                _logger.LogError(ex, "All retry attempts failed for updating strain with Name {Name}", strain.Name);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Updating Strain Of Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }

        }

        public async Task AddGeneToGraph(Gene gene)
        {
            _logger.LogInformation("AddGeneToGraph(): Adding gene with id {id} strainId {strainId} accession number {accessionNumber} and name {name} and function {function} and product {product} and functional category {functionalCategory}", gene.GeneId, gene.StrainId, gene.AccessionNumber, gene.Name, gene.Function, gene.Product, gene.FunctionalCategory);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var createGeneQuery = @"
                                MERGE (g:Gene {accessionNumber: $accessionNumber})
                                ON CREATE SET g.geneId = $geneId, g.name = $name, g.function = $function, g.product = $product

                                WITH g
                                FOREACH (ignoreMe IN CASE WHEN $functionalCategory IS NOT NULL AND $functionalCategory <> '' THEN [1] ELSE [] END |
                                    MERGE (fc:FunctionalCategory {name: $functionalCategory})
                                    MERGE (g)-[:BELONGS_TO]->(fc)
                                )

                                WITH g
                                MERGE (s:Strain {strainId: $strainId})
                                MERGE (g)-[:PART_OF]->(s)
                            ";

                        _logger.LogInformation("tx.RunAsync Adding gene with accession number {accessionNumber}", gene.AccessionNumber);
                        await tx.RunAsync(createGeneQuery, new
                        {
                            geneId = gene.GeneId,
                            strainId = gene.StrainId,
                            accessionNumber = gene.AccessionNumber,
                            name = gene.Name,
                            function = gene.Function,
                            product = gene.Product,
                            functionalCategory = gene.FunctionalCategory
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddGeneToGraph");
                _logger.LogError(ex, "All retry attempts failed for adding gene with accession number {AccessionNumber}", gene.AccessionNumber);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Adding Gene To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateGeneOfGraph(Gene gene)
        {
            _logger.LogInformation("UpdateGeneOfGraph(): Updating gene with id {id} strainId {strainId} accession number {accessionNumber} and name {name} and function {function} and product {product} and functional category {functionalCategory}", gene.GeneId, gene.StrainId, gene.AccessionNumber, gene.Name, gene.Function, gene.Product, gene.FunctionalCategory);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {

                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var updateGeneQuery = @"
                                MATCH (g:Gene {geneId: $geneId})
                                SET g.name = $name, g.function = $function, g.product = $product

                                WITH g
                                OPTIONAL MATCH (g)-[r1:BELONGS_TO]->(oldFc:FunctionalCategory)
                                WHERE (oldFc IS NULL AND $functionalCategory IS NOT NULL) OR (oldFc IS NOT NULL AND oldFc.name <> $functionalCategory)
                                DELETE r1

                                WITH g
                                FOREACH (ignoreMe IN CASE WHEN $functionalCategory IS NOT NULL THEN [1] ELSE [] END |
                                    MERGE (newFc:FunctionalCategory {name: $functionalCategory})
                                    MERGE (g)-[:BELONGS_TO]->(newFc)
                                )

                                WITH g
                                OPTIONAL MATCH (g)-[r2:PART_OF]->(oldStrain:Strain)
                                WHERE oldStrain.strainId <> $strainId OR oldStrain IS NULL
                                DELETE r2
                                
                                WITH g
                                MERGE (newStrain:Strain {strainId: $strainId})
                                MERGE (g)-[:PART_OF]->(newStrain)";

                        _logger.LogInformation("tx.RunAsync Updating gene with accession number {accessionNumber}", gene.AccessionNumber);
                        await tx.RunAsync(updateGeneQuery, new
                        {
                            geneId = gene.GeneId,
                            strainId = gene.StrainId,
                            accessionNumber = gene.AccessionNumber,
                            name = gene.Name,
                            function = gene.Function,
                            product = gene.Product,
                            functionalCategory = gene.FunctionalCategory
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateGeneOfGraph");
                _logger.LogError(ex, "All retry attempts failed for updating gene with accession number {AccessionNumber}", gene.AccessionNumber);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Updating Gene Of Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }

        }

        // Delete a gene from the graph database
        public async Task DeleteGeneFromGraph(string geneId)
        {
            _logger.LogInformation("DeleteGeneFromGraph(): Deleting gene with accession number {geneId}", geneId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var deleteGeneQuery = @"
                            MATCH (g:Gene {geneId: $geneId})
                            DETACH DELETE g
                        ";

                        _logger.LogInformation("tx.RunAsync Deleting gene with accession number {geneId}", geneId);
                        await tx.RunAsync(deleteGeneQuery, new
                        {
                            geneId = geneId
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteGeneFromGraph");
                _logger.LogError(ex, "All retry attempts failed for deleting gene with accession number {geneId}", geneId);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Deleting Gene From Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }



        // Define the RetryPolicy
        /*
        This method call specifies the type of exception that the policy should handle, which in this case is ClientException.
        The lambda expression ex => ex.Message.Contains("ConstraintValidationFailed") further filters these exceptions to only 
        those where the exception's message contains the text "ConstraintValidationFailed". 
        This is likely a specific error message you expect from Neo4j when a unique constraint is violated.

        The need for this retry policy is because multiple nodes of same functional category were created in the graph database
        when uploading in bulk.
        */
        private static readonly Func<ILogger<GraphRepositoryForGene>, IAsyncPolicy> CreateRetryPolicy = logger => Policy
            .Handle<ClientException>(ex => ex.Message.Contains("ConstraintValidationFailed"))
            .WaitAndRetryAsync(
                new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(7)
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning("Attempt {RetryCount} failed with exception. Waiting {TimeSpan} before next retry. Exception: {Exception}",
                        retryCount, timeSpan, exception.Message);
                }
            );
    }
}
