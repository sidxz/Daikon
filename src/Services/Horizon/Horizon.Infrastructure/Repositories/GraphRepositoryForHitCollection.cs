
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Screens;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Polly;

namespace Horizon.Infrastructure.Repositories
{
    public class GraphRepositoryForHitCollection : IGraphRepositoryForHitCollection
    {
        private readonly IDriver _driver;
        private ILogger<GraphRepositoryForHitCollection> _logger;

        public GraphRepositoryForHitCollection(IDriver driver, ILogger<GraphRepositoryForHitCollection> logger)
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
                    var createIndexQuery = "CREATE INDEX hit_collection_id_index IF NOT EXISTS FOR (h:HitCollection) ON (h.hitCollectionId);";
                    await tx.RunAsync(createIndexQuery);

                    createIndexQuery = "CREATE INDEX hit_collection_name_index IF NOT EXISTS FOR (h:HitCollection) ON (h.name);";
                    await tx.RunAsync(createIndexQuery);

                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }




        public async Task AddHitCollectionToGraph(HitCollection hitCollection)
        {
            _logger.LogInformation("AddHitCollectionToGraph(): Adding hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);

                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var createHitCollectionQuery = @"
                            CREATE (h:HitCollection {hitCollectionId: $hitCollectionId, name:  $name, 
                                                        hitCollectionType: $hitCollectionType, strainId: $strainId, 
                                                        primaryOrgName: $primaryOrgName
                                                    })
                  ";

                        await tx.RunAsync(createHitCollectionQuery, new
                        {
                            hitCollectionId = hitCollection.HitCollectionId,
                            name = hitCollection.Name,
                            hitCollectionType = hitCollection.HitCollectionType,
                            strainId = hitCollection.StrainId,
                            primaryOrgName = hitCollection.PrimaryOrgName
                        });
                        if (hitCollection.ScreenId != null)
                        {
                            var relateToScreenQuery = @"
                                MATCH (s:Screen {screenId: $screenId})
                                MATCH (h:HitCollection {hitCollectionId: $hitCollectionId})
                                MERGE (h)-[:HIT_COLLECTION_OF]->(s)
                            ";
                            await tx.RunAsync(relateToScreenQuery, new
                            {
                                screenId = hitCollection.ScreenId,
                                hitCollectionId = hitCollection.HitCollectionId
                            });
                        }

                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddHitCollectionToGraph");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Adding HitCollection To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateHitCollectionOfGraph(HitCollection hitCollection)
        {
            _logger.LogInformation("AddHitCollectionToGraph(): Adding hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var updateHitCollectionQuery = @"
                            MATCH (h:HitCollection {hitCollectionId: $hitCollectionId})
                            SET h.name: $name, h.hitCollectionType: $hitCollectionType,
                                h.strainId: $strainId, h.primaryOrgName: $primaryOrgName
                    ";
                        await tx.RunAsync(updateHitCollectionQuery, new
                        {
                            hitCollectionId = hitCollection.HitCollectionId,
                            name = hitCollection.Name,
                            hitCollectionType = hitCollection.HitCollectionType,
                            strainId = hitCollection.StrainId,
                            primaryOrgName = hitCollection.PrimaryOrgName
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHitCollectionOfGraph");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Updating HitCollection In Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateAssociatedScreenOfHitCollection(HitCollection hitCollection)
        {
            _logger.LogInformation("UpdateAssociatedScreenOfHitCollection(): Updating hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var deleteAssociatedScreenQuery = @"
                            MATCH (s:HitCollection {hitCollectionId: $hitCollectionId})-[r:HIT_COLLECTION_OF]->(t:Screen)
                            DELETE r
                            ";
                        await tx.RunAsync(deleteAssociatedScreenQuery, new
                        {
                            hitCollectionId = hitCollection.HitCollectionId,
                        });

                        if (hitCollection.ScreenId != null)
                        {
                            var relateToScreenQuery = @"
                                MATCH (s:Screen {screenId: $screenId})
                                MATCH (h:HitCollection {hitCollectionId: $hitCollectionId})
                                MERGE (h)-[:HIT_COLLECTION_OF]->(s)
                            ";
                            await tx.RunAsync(relateToScreenQuery, new
                            {
                                screenId = hitCollection.ScreenId,
                                hitCollectionId = hitCollection.HitCollectionId
                            });
                        }

                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAssociatedScreenOfHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Updating Associated Screens Of Hit Collection In Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public Task DeleteHitCollectionFromGraph(string hitCollectionId)
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
        private static readonly Func<ILogger<GraphRepositoryForHitCollection>, IAsyncPolicy> CreateRetryPolicy = logger => Policy
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
