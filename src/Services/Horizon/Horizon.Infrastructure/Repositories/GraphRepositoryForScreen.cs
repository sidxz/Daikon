
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Screens;
using Horizon.Domain.Targets;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Polly;

namespace Horizon.Infrastructure.Repositories
{
    public class GraphRepositoryForScreen : IGraphRepositoryForScreen
    {
        private readonly IDriver _driver;
        private ILogger<GraphRepositoryForScreen> _logger;

        public GraphRepositoryForScreen(IDriver driver, ILogger<GraphRepositoryForScreen> logger)
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
                    var createIndexQuery = "CREATE INDEX screen_id_index IF NOT EXISTS FOR (s:Screen) ON (s.screenId);";
                    await tx.RunAsync(createIndexQuery);

                    createIndexQuery = "CREATE INDEX screen_name_index IF NOT EXISTS FOR (s:Screen) ON (s.name);";
                    await tx.RunAsync(createIndexQuery);

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }




        public async Task AddScreenToGraph(Screen screen)
        {
            _logger.LogInformation("AddScreenToGraph(): Adding screen with id {ScreenId} and name {Name} and targets {targets}", screen.ScreenId, screen.Name, screen.AssociatedTargetsId.ToString());
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);

                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var createScreenQuery = @"
                            CREATE (s:Screen {screenId: $screenId, name: $name, screenType: $screenType, method: $method, status: $status, primaryOrgName: $primaryOrgName})
                        ";

                        await tx.RunAsync(createScreenQuery, new
                        {
                            screenId = screen.ScreenId,
                            name = screen.Name,
                            screenType = screen.ScreenType,
                            method = screen.Method,
                            status = screen.Status,
                            primaryOrgName = screen.PrimaryOrgName
                        });

                        foreach (var targetId in screen.AssociatedTargetsId)
                        {
                            var relateToTargetQuery = @"
                                MATCH (s:Screen {screenId: $screenId})
                                MATCH (t:Target {targetId: $targetId})
                                MERGE (s)-[:SCREENS]->(t)
                            ";

                            await tx.RunAsync(relateToTargetQuery, new
                            {
                                screenId = screen.ScreenId,
                                targetId = targetId
                            });
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddScreenToGraph");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Adding Screen To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateScreenOfGraph(Screen screen)
        {
            _logger.LogInformation("UpdateScreenOfGraph(): Updating screen with id {ScreenId} and name {Name} and targets {targets}", screen.ScreenId, screen.Name, screen.AssociatedTargetsId.ToString());
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var updateScreenQuery = @"
                            MATCH (s:Screen {screenId: $screenId})
                            SET s.name = $name, s.screenType = $screenType, s.method = $method, s.status = $status, s.primaryOrgName = $primaryOrgName
                    ";
                        await tx.RunAsync(updateScreenQuery, new
                        {
                            screenId = screen.ScreenId,
                            name = screen.Name,
                            screenType = screen.ScreenType,
                            method = screen.Method,
                            status = screen.Status,
                            primaryOrgName = screen.PrimaryOrgName
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateScreenOfGraph");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Updating Screen In Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateAssociatedTargetsOfScreen(Screen screen)
        {
            _logger.LogInformation("UpdateAssociatedTargetsOfScreen(): Updating screen with id {ScreenId} and name {Name} and targets {targets}", screen.ScreenId, screen.Name, screen.AssociatedTargetsId.ToString());
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var deleteAssociatedTargetsQuery = @"
                            MATCH (s:Screen {screenId: $screenId})-[r:SCREENS]->(t:Target)
                            DELETE r
                            ";
                        await tx.RunAsync(deleteAssociatedTargetsQuery, new
                        {
                            screenId = screen.ScreenId,
                        });

                        var updateAssociatedTargetsQuery = @"
                            
                            MATCH (s:Screen {screenId: $screenId})

                            WITH s, CASE WHEN $associatedTargets IS NULL THEN [] ELSE $associatedTargets END AS safeTargets
                            UNWIND safeTargets AS targetId

                            MATCH (t:Target {targetId: targetId})
                            MERGE (s)-[:SCREENS]->(t)

                            ";
                        await tx.RunAsync(updateAssociatedTargetsQuery, new
                        {
                            screenId = screen.ScreenId,
                            associatedTargets = screen.AssociatedTargetsId
                        });

                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAssociatedTargetsOfScreen");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Updating Associated Genes Of Target In Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public Task DeleteScreenFromGraph(string screenId)
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
        private static readonly Func<ILogger<GraphRepositoryForScreen>, IAsyncPolicy> CreateRetryPolicy = logger => Policy
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
