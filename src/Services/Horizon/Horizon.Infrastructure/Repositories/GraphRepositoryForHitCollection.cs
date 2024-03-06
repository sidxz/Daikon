
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




        public async Task AddHitCollection(HitCollection hitCollection)
        {
            _logger.LogInformation("AddHitCollection(): Adding hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);
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
                _logger.LogError(ex, "Error in AddHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Adding HitCollection To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateHitCollection(HitCollection hitCollection)
        {
            _logger.LogInformation("AddHitCollection(): Adding hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);
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
                _logger.LogError(ex, "Error in UpdateHitCollection");
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

        public Task DeleteHitCollection(string hitCollectionId)
        {
            _logger.LogInformation("DeleteHitCollection(): Deleting hitCollection with id {HitCollectionId}", hitCollectionId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                return retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var deleteHitCollectionQuery = @"
                            MATCH (h:HitCollection {hitCollectionId: $hitCollectionId})
                            DETACH DELETE h
                        ";
                        await tx.RunAsync(deleteHitCollectionQuery, new
                        {
                            hitCollectionId = hitCollectionId
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Deleting HitCollection From Graph", ex);
            }
            finally
            {
                session.CloseAsync();
            }
        }

        public Task RenameHitCollection(string hitCollectionId, string newName)
        {
            _logger.LogInformation("RenameHitCollection(): Renaming hitCollection with id {HitCollectionId} to {NewName}", hitCollectionId, newName);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                return retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var renameHitCollectionQuery = @"
                            MATCH (h:HitCollection {hitCollectionId: $hitCollectionId})
                            SET h.name = $newName
                        ";
                        await tx.RunAsync(renameHitCollectionQuery, new
                        {
                            hitCollectionId = hitCollectionId,
                            newName = newName
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RenameHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Renaming HitCollection In Graph", ex);
            }
            finally
            {
                session.CloseAsync();
            }
        }

        public Task AddHit(Hit hit)
        {
            _logger.LogInformation("AddHit(): Adding hit with id {HitId} and MoleculeId {MoleculeId} MoleculeRegId {MoleculeRegId} and hitCollectionId {hitCollectionId}", hit.HitId, hit.MoleculeId, hit.MoleculeRegistrationId, hit.HitCollectionId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                return retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        // var createHitQuery = @"
                        //     MATCH (h:HitCollection {hitCollectionId: $hitCollectionId})
                        //     CREATE (h)-[:HIT]->(hit:Hit {hitId: $hitId, library: $library, requestedSMILES: $requestedSMILES})
                        // ";

                        var createHitQuery = @"
                            MATCH (hc:HitCollection {hitCollectionId: $hitCollectionId})
                            MERGE (hc)-[:HIT]->(hit:Hit {hitId: $hitId, library: $library, requestedSMILES: $requestedSMILES})
                            WITH hit
                            FOREACH (_ IN CASE WHEN $moleculeRegistrationId IS NOT NULL AND $moleculeRegistrationId <> '' THEN [1] ELSE [] END |
                                MERGE (m:Molecule {registrationId: $moleculeRegistrationId})
                                MERGE (hit)-[:HIT_MOLECULE]->(m)
                            )
                        ";
                        await tx.RunAsync(createHitQuery, new
                        {
                            hitCollectionId = hit.HitCollectionId,
                            hitId = hit.HitId,
                            library = hit.Library,
                            requestedSMILES = hit.RequestedSMILES,
                            moleculeRegistrationId = hit.MoleculeRegistrationId,
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Adding Hit To Graph", ex);
            }
            finally
            {
                session.CloseAsync();
            }
        }

        public Task UpdateHit(Hit hit)
        {
            _logger.LogInformation("UpdateHit(): Updating hit with id {HitId} and MoleculeId {MoleculeID} and hitCollectionId {hitCollectionId}", hit.HitId, hit.MoleculeId, hit.HitCollectionId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                return retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var updateHitQuery = @"
                            MATCH (h:HitCollection {hitCollectionId: $hitCollectionId})-[:HIT]->(hit:Hit {hitId: $hitId})
                            SET hit.requestedSMILES = $requestedSMILES
                        ";
                        await tx.RunAsync(updateHitQuery, new
                        {
                            hitCollectionId = hit.HitCollectionId,
                            hitId = hit.HitId,
                            requestedSMILES = hit.RequestedSMILES
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Updating Hit In Graph", ex);
            }
            finally
            {
                session.CloseAsync();
            }
        }

        public Task DeleteHit(string hitId)
        {
            _logger.LogInformation("DeleteHit(): Deleting hit with id {HitId}", hitId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);
                return retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var deleteHitQuery = @"
                            MATCH (h:HitCollection)-[:HIT]->(hit:Hit {hitId: $hitId})
                            DETACH DELETE hit
                        ";
                        await tx.RunAsync(deleteHitQuery, new
                        {
                            hitId = hitId
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Deleting Hit From Graph", ex);
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
