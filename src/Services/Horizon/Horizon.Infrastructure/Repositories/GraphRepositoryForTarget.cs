
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Targets;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Polly;

namespace Horizon.Infrastructure.Repositories
{
    public class GraphRepositoryForTarget : IGraphRepositoryForTarget
    {
        private readonly IDriver _driver;
        private ILogger<GraphRepositoryForTarget> _logger;

        public GraphRepositoryForTarget(IDriver driver, ILogger<GraphRepositoryForTarget> logger)
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
                    var createIndexQuery = "CREATE INDEX target_name_index IF NOT EXISTS FOR (t:Target) ON (t.name);";
                    await tx.RunAsync(createIndexQuery);

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }




        public async Task AddTargetToGraph(Target target)
        {
            _logger.LogInformation("AddTargetToGraph(): Adding target with id {TargetId} and name {Name}", target.TargetId, target.Name);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);

                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var createTargetQuery = @"
                            CREATE (t:Target {targetId: $targetId, name: $name, targetType: $targetType, bucket: $bucket})
                        ";

                        await tx.RunAsync(createTargetQuery, new
                        {
                            targetId = target.TargetId,
                            name = target.Name,
                            targetType = target.TargetType,
                            bucket = target.Bucket
                        });

                        foreach (var accessionNumber in target.GeneAccessionNumbers)
                        {
                            var relateToGeneQuery = @"
                                MATCH (g:Gene {accessionNumber: $accessionNumber})
                                MATCH (t:Target {targetId: $targetId})
                                MERGE (t)-[:TARGETS {targetType: $targetType }]->(g)
                            ";

                            await tx.RunAsync(relateToGeneQuery, new
                            {
                                accessionNumber = accessionNumber,
                                targetId = target.TargetId,
                                targetType = target.TargetType,
                            });
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddTargetToGraph");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Adding Target To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public Task UpdateTargetOfGraph(Target target)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTargetFromGraph(string targetId)
        {
            throw new NotImplementedException();
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
        private static readonly Func<ILogger<GraphRepositoryForTarget>, IAsyncPolicy> CreateRetryPolicy = logger => Policy
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
