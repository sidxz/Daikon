using System;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
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

        public async Task AddStrainToGraph(string name, string strainId, string organism)
        {
            _logger.LogInformation("AddStrainToGraph(): Adding strain with name {Name} and id {StrainId}", name, strainId);
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


                        _logger.LogInformation("tx.RunAsync Adding strain with name {Name}", name);
                        await tx.RunAsync(createStrainQuery, new
                        {
                            name,
                            strainId,
                            organism
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddStrainToGraph");
                _logger.LogError(ex, "All retry attempts failed for adding strain with Name {Name}", name);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Adding Strain To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task AddGeneToGraph(string accessionNumber, string name, string function, string product, string functionalCategory)
        {
            _logger.LogInformation("AddGeneToGraph(): Adding gene with accession number {accessionNumber} and name {name} and function {function} and product {product} and functional category {functionalCategory}", accessionNumber, name, function, product, functionalCategory);
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
                    ON CREATE SET g.name = $name, g.function = $function, g.product = $product
                    WITH g
                    MERGE (fc:FunctionalCategory {name: $functionalCategory})
                    MERGE (g)-[:BELONGS_TO]->(fc)
                ";

                        if (string.IsNullOrWhiteSpace(functionalCategory))
                        {
                            createGeneQuery = @"
                        MERGE (g:Gene {accessionNumber: $accessionNumber})
                        ON CREATE SET g.name = $name, g.function = $function, g.product = $product
                    ";
                        }

                        _logger.LogInformation("tx.RunAsync Adding gene with accession number {accessionNumber}", accessionNumber);
                        _logger.LogDebug("Adding gene with accession number {accessionNumber} and name {name} and function {function} and product {product} and functional category {functionalCategory}", accessionNumber, name, function, product, functionalCategory);
                        await tx.RunAsync(createGeneQuery, new
                        {
                            accessionNumber,
                            name,
                            function,
                            product,
                            functionalCategory
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddGeneToGraph");
                _logger.LogError(ex, "All retry attempts failed for adding gene with accession number {AccessionNumber}", accessionNumber);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Adding Gene To Graph", ex);
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
