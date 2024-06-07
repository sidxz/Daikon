
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Screens;
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

            try
            {
                // var query = @"
                //   CREATE INDEX screen_uniId_index IF NOT EXISTS FOR (s:Screen) ON (s.uniId);
                // ";
                // var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateIndexesAsync");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Creating Indexes In Graph", ex);
            }
        }

        public async Task CreateConstraintsAsync()
        {
            try
            {
                var query = @"
                    CREATE CONSTRAINT screen_uniId_unique IF NOT EXISTS FOR (s:Screen) REQUIRE s.uniId IS UNIQUE;
                ";
                var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateConstraintsAsync");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Creating Constraints In Graph", ex);
            }
        }


        public async Task AddScreen(Screen screen)
        {
            _logger.LogInformation("AddScreen(): Adding screen with id {ScreenId} and name {Name} and targets {targets}", screen.ScreenId, screen.Name, screen.AssociatedTargetsId.ToString());
            try
            {
                var query = @"
                   MERGE (s:Screen { uniId: $uniId })
                            ON CREATE SET
                                        s.name = $name,
                                        s.screenType = $screenType,
                                        s.method = $method,
                                        s.status = $status,
                                        s.primaryOrgId = $primaryOrgId
                            ON MATCH SET  
                                        s.name = $name,
                                        s.screenType = $screenType,
                                        s.method = $method,
                                        s.status = $status,
                                        s.primaryOrgId = $primaryOrgId
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = screen.UniId,
                                 name = screen.Name,
                                 screenType = screen.ScreenType,
                                 method = screen.Method,
                                 status = screen.Status,
                                 primaryOrgId = screen.PrimaryOrgId
                             }).ExecuteAsync()
                             ;

                foreach (var targetId in screen.AssociatedTargetsId)
                {
                    var relateQuery = @"
                                MATCH (s:Screen {uniId: $_screenId})
                                MATCH (t:Target {uniId: $_targetId})
                                MERGE (s)-[:SCREENS]->(t)
                                ";

                    var (query2Results, _) = await _driver
                                 .ExecutableQuery(relateQuery).WithParameters(new
                                 {
                                     _screenId = screen.ScreenId,
                                     _targetId = targetId
                                 }).ExecuteAsync()
                                 ;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddScreen");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Adding Screen To Graph", ex);
            }
        }

        public async Task UpdateScreen(Screen screen)
        {
            _logger.LogInformation("UpdateScreen(): Updating screen with id {ScreenId} and name {Name} and targets {targets}", screen.ScreenId, screen.Name, screen.AssociatedTargetsId.ToString());
            try
            {

                var query = @"
                   MATCH (s:Screen {uniId: $uniId})
                            SET 
                                s.name = $name,
                                s.screenType = $screenType, 
                                s.method = $method, 
                                s.status = $status, 
                                s.primaryOrgId = $primaryOrgId
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = screen.ScreenId,
                                 name = screen.Name,
                                 screenType = screen.ScreenType,
                                 method = screen.Method,
                                 status = screen.Status,
                                 primaryOrgId = screen.PrimaryOrgId
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateScreen");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Updating Screen In Graph", ex);
            }
        }

        public async Task UpdateAssociatedTargetsOfScreen(Screen screen)
        {
            _logger.LogInformation("UpdateAssociatedTargetsOfScreen(): Updating screen with id {ScreenId} and name {Name} and targets {targets}", screen.ScreenId, screen.Name, screen.AssociatedTargetsId.ToString());
            try
            {

                // delete previous associations
                var query = @"
                    MATCH (s:Screen {uniId: $uniId})-[r:SCREENS]->(t:Target)
                            DELETE r
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = screen.ScreenId,
                             }).ExecuteAsync()
                             ;

                // create new associations

                var query2 = @"
                    MATCH (s:Screen {uniId: $uniId})
                    WITH s, 
                        CASE WHEN $associatedTargets IS NULL THEN [] ELSE $associatedTargets END AS safeTargets
                            UNWIND safeTargets AS targetId
                                MATCH (t:Target {uniId: targetId})
                                MERGE (s)-[:SCREENS]->(t)
                ";
                var (queryResults2, _) = await _driver
                             .ExecutableQuery(query2).WithParameters(new
                             {
                                 uniId = screen.ScreenId,
                                 associatedTargets = screen.AssociatedTargetsId
                             }).ExecuteAsync()
                             ;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAssociatedTargetsOfScreen");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Updating Associated Genes Of Target In Graph", ex);
            }
        }

        public async Task DeleteScreen(string screenId)
        {
            _logger.LogInformation("DeleteScreen(): Deleting screen with id {ScreenId}", screenId);

            try
            {
                var query = @"
                     MATCH (s:Screen {uniId: $uniId})
                        DETACH DELETE s
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = screenId,
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteScreen");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Deleting Screen From Graph", ex);
            }
        }

        public async Task RenameScreen(string screenId, string newName)
        {
            _logger.LogInformation("RenameScreen(): Renaming screen with id {ScreenId} to {NewName}", screenId, newName);

            try
            {
                var query = @"
                    MATCH (s:Screen {uniId: $uniId})
                        SET s.name = $_newName
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = screenId,
                                 _newName = newName
                             }).ExecuteAsync()
                             ;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RenameScreen");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Renaming Screen In Graph", ex);
            }
        }
    }
}
