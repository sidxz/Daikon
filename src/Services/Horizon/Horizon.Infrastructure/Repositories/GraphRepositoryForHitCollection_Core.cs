
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Screens;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Polly;

namespace Horizon.Infrastructure.Repositories
{
    public partial class GraphRepositoryForHitCollection : IGraphRepositoryForHitCollection
    {
        private readonly IDriver _driver;
        private ILogger<GraphRepositoryForHitCollection> _logger;
        private readonly QueryConfig _queryConfig;

        public GraphRepositoryForHitCollection(IDriver driver, ILogger<GraphRepositoryForHitCollection> logger)
        {
            _driver = driver;
            _logger = logger;
            _queryConfig = new QueryConfig(database: Environment.GetEnvironmentVariable("Database") ?? "Horizon");
        }

        public async Task CreateIndexesAsync()
        {
            try
            {
                var query = @"
                  CREATE INDEX hit_collection_uniId_index IF NOT EXISTS FOR (hc:HitCollection) ON (hc.uniId);
                ";
                var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();

                var query2 = @"
                  CREATE INDEX hit_index IF NOT EXISTS FOR (h:Hit) ON (h.uniId);
                ";
                var (query2Results, _) = await _driver.ExecutableQuery(query2).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateIndexesAsync");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Creating Indexes In Graph", ex);
            }
        }


        public async Task CreateConstraintsAsync()
        {
            
        }

        public async Task AddHitCollection(HitCollection hitCollection)
        {
            _logger.LogInformation("AddHitCollection(): Adding hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);

            try
            {
                var query = @"
                    MERGE (h:HitCollection {uniId: $uniId})
                        ON CREATE SET 
                            h.name = $name, 
                            h.hitCollectionType = $hitCollectionType,
                            h.strainId = $strainId,
                            h.primaryOrgName = $primaryOrgName
                        ON MATCH SET
                            h.name = $name, 
                            h.hitCollectionType = $hitCollectionType,
                            h.strainId = $strainId,
                            h.primaryOrgName = $primaryOrgName
                        WITH h
                        FOREACH (ignoreMe IN CASE WHEN $screenId IS NOT NULL THEN [1] ELSE [] END |
                            MERGE (s:Screen {uniId: $screenId})
                            MERGE (h)-[:HIT_COLLECTION_OF]->(s)
                    )
                ";
                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = hitCollection.UniId,
                        name = hitCollection.Name,
                        hitCollectionType = hitCollection.HitCollectionType,
                        strainId = hitCollection.StrainId,
                        primaryOrgName = hitCollection.PrimaryOrgName,
                        screenId = hitCollection.ScreenId
                    }).ExecuteAsync()
                    ;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Adding HitCollection To Graph", ex);
            }
        }


        public async Task UpdateHitCollection(HitCollection hitCollection)
        {
            _logger.LogInformation("AddHitCollection(): Adding hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);

            try
            {
                var query = @"
                   MATCH (h:HitCollection {uniId: $uniId})
                        SET 
                            h.name: $name,
                            h.hitCollectionType: $hitCollectionType,
                            h.strainId: $strainId,
                            h.primaryOrgName: $primaryOrgName
                    )
                ";
                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = hitCollection.UniId,
                        name = hitCollection.Name,
                        hitCollectionType = hitCollection.HitCollectionType,
                        strainId = hitCollection.StrainId,
                        primaryOrgName = hitCollection.PrimaryOrgName
                    }).ExecuteAsync()
                    ;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Updating HitCollection In Graph", ex);
            }
        }

        public async Task UpdateAssociatedScreenOfHitCollection(HitCollection hitCollection)
        {
            _logger.LogInformation("UpdateAssociatedScreenOfHitCollection(): Updating hitCollection with id {HitCollectionId} and name {Name} and screenId {screenId}", hitCollection.HitCollectionId, hitCollection.Name, hitCollection.ScreenId);
            try
            {
                // delete the existing relationship
                var query = @"
                   MATCH (s:HitCollection {uniId: $uniId})-[r:HIT_COLLECTION_OF]->(t:Screen)
                   DELETE r
                    )
                ";
                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = hitCollection.HitCollectionId,
                    }).ExecuteAsync()
                    ;

                // create the new relationship
                var query2 = @"
                    MATCH (s:Screen {uniId: $screenId})
                    MATCH (h:HitCollection {uniId: $uniId})
                    MERGE (h)-[:HIT_COLLECTION_OF]->(s)
                ";
                var (query2Results, _) = await _driver
                   .ExecutableQuery(query2).WithParameters(new
                   {
                       uniId = hitCollection.HitCollectionId,
                       screenId = hitCollection.ScreenId,
                   }).ExecuteAsync()
                   ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAssociatedScreenOfHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Updating Associated Screens Of Hit Collection In Graph", ex);
            }
        }

        public async Task DeleteHitCollection(string hitCollectionId)
        {
            _logger.LogInformation("DeleteHitCollection(): Deleting hitCollection with id {HitCollectionId}", hitCollectionId);

            try
            {
                var query = @"
                  MATCH (h:HitCollection {uniId: $uniId})
                  DETACH DELETE h
                    )
                ";
                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = hitCollectionId
                    }).ExecuteAsync()
                    ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Deleting HitCollection From Graph", ex);
            }
        }

        public async Task RenameHitCollection(string hitCollectionId, string newName)
        {
            _logger.LogInformation("RenameHitCollection(): Renaming hitCollection with id {HitCollectionId} to {NewName}", hitCollectionId, newName);

            try
            {
                var query = @"
                   MATCH (h:HitCollection {uniId: $uniId})
                    SET 
                        h.name = $_newName
                    )
                ";
                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = hitCollectionId,
                        _newName = newName
                    }).ExecuteAsync()
                    ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RenameHitCollection");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Renaming HitCollection In Graph", ex);
            }
        }
    }
}
