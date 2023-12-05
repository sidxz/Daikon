using System;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Infrastructure.Repositories
{
    public class GraphRepository : IGraphRepository
    {
        private readonly IDriver _driver;
        private ILogger<GraphRepository> _logger;

        public GraphRepository(IDriver driver, ILogger<GraphRepository> logger)
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

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }


        public async Task AddGeneToGraph(string accessionNumber, string name, string function, string product, string functionalCategory)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    var createGeneQuery = @"
                        MERGE (g:Gene {accessionNumber: $accessionNumber})
                        ON CREATE SET g.name = $name, g.function = $function, g.product = $product
                        MERGE (fc:FunctionalCategory {name: $functionalCategory})
                        MERGE (g)-[:BELONGS_TO]->(fc)
                    ";

                    if (string.IsNullOrWhiteSpace(functionalCategory))
                        createGeneQuery = @"
                        MERGE (g:Gene {accessionNumber: $accessionNumber})
                        ON CREATE SET g.name = $name, g.function = $function, g.product = $product
                    ";

                    _logger.LogInformation("Adding gene with accession number {accessionNumber}", accessionNumber);
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
            }
            catch (ClientException ex)
            {
                _logger.LogError(ex, "ClientException :Error in AddGeneToGraph");
               throw new RepositoryException(nameof(GraphRepository), "Error Adding Gene To Graph", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddGeneToGraph");
                throw new RepositoryException(nameof(GraphRepository), "Error Adding Gene To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
