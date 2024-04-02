
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
                    var createIndexQuery = "CREATE INDEX target_id_index IF NOT EXISTS FOR (t:Target) ON (t.targetId);";
                    await tx.RunAsync(createIndexQuery);

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }




        public async Task AddTarget(Target target)
        {
            _logger.LogInformation("AddTarget(): Adding target with id {TargetId} and name {Name} and genes {genes}", target.TargetId, target.Name, target.GeneAccessionNumbers.ToString());
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);

                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var createTargetQuery = @"
                            CREATE (t:Target { uniId: $uniId, targetId: $targetId, name: $name, targetType: $targetType, associatedGenes: $associatedGenes, bucket: $bucket})
                        ";

                        await tx.RunAsync(createTargetQuery, new
                        {
                            uniId = target.UniId,
                            targetId = target.TargetId,
                            name = target.Name,
                            targetType = target.TargetType,
                            associatedGenes = target.GeneAccessionNumbers,
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
                _logger.LogError(ex, "Error in AddTarget");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Adding Target To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateTarget(Target target)
        {
            _logger.LogInformation("UpdateTarget(): Updating target with id {TargetId}", target.TargetId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var updateTargetQuery = @"
                    MATCH (t:Target {targetId: $targetId})
                    SET t.targetType = $targetType, t.bucket = $bucket
                    ";
                        await tx.RunAsync(updateTargetQuery, new
                        {
                            targetId = target.TargetId,
                            targetType = target.TargetType,
                            bucket = target.Bucket
                        });

                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateTarget");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Updating Target In Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateAssociatedGenesOfTarget(Target target)
        {
            _logger.LogInformation("UpdateAssociatedGenesOfTargetInGraph(): Updating associated genes of target with id {TargetId}", target.TargetId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var deleteAssociatedGenesQuery = @"
                            MATCH (t:Target {targetId: $targetId})-[r:TARGETS]->(g:Gene)
                            DELETE r
                            ";
                        await tx.RunAsync(deleteAssociatedGenesQuery, new
                        {
                            targetId = target.TargetId
                        });

                        var updateTargetQuery = @"
                            MATCH (t:Target {targetId: $targetId})
                            SET t.associatedGenes = $associatedGenes
                            ";
                        await tx.RunAsync(updateTargetQuery, new
                        {
                            targetId = target.TargetId,
                            associatedGenes = target.GeneAccessionNumbers
                        });



                        foreach (var accessionNumber in target.GeneAccessionNumbers)
                        {
                            var relateToGeneQuery = @"
                                MATCH (g:Gene {accessionNumber: $_accessionNumber})
                                MATCH (t:Target {targetId: $targetId})
                                MERGE (t)-[:TARGETS {targetType: $targetType }]->(g)
                            ";

                            await tx.RunAsync(relateToGeneQuery, new
                            {
                                _accessionNumber = accessionNumber,
                                targetId = target.TargetId,
                                targetType = target.TargetType,
                            });
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAssociatedGenesOfTargetInGraph");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Updating Associated Genes Of Target In Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public Task DeleteTarget(string targetId)
        {
            throw new NotImplementedException();
        }

        public Task RenameTarget(string targetId, string newName)
        {
            _logger.LogInformation("RenameTarget(): Renaming target with id {targetId} to {NewName}", targetId, newName);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                return retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var renameScreenQuery = @"
                            MATCH (t:Target {targetId: $_targetId})
                            SET t.name = $_newName
                        ";
                        await tx.RunAsync(renameScreenQuery, new
                        {
                            _targetId = targetId,
                            _newName = newName
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Rename target");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Renaming Target In Graph", ex);
            }
            finally
            {
                session.CloseAsync();
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
