
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistence;
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
            try
            {
                // var query = @"
                //   CREATE INDEX target_uniId_index IF NOT EXISTS FOR (t:Target) ON (t.uniId);
                // ";
                // var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateIndexesAsync");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Creating Indexes In Graph", ex);
            }
        }

        public async Task CreateConstraintsAsync()
        {
            try
            {
                var query = @"
                    CREATE CONSTRAINT target_uniId_unique IF NOT EXISTS FOR (t:Target) REQUIRE t.uniId IS UNIQUE;
                ";
                var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateConstraintsAsync");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Creating Constraints In Graph", ex);
            }
        }

        public async Task AddTarget(Target target)
        {
            _logger.LogInformation("AddTarget(): Adding target with id {TargetId} and name {Name} and genes {genes}", target.TargetId, target.Name, target.GeneAccessionNumbers.ToString());

            try
            {

                var query = @"
                   MERGE (t:Target { uniId: $uniId })
                            ON CREATE SET
                                        t.name = $name,
                                        t.targetType = $targetType,
                                        t.associatedGenes = $associatedGenes,
                                        t.bucket = $bucket
                            ON MATCH SET  
                                        t.name = $name,
                                        t.targetType = $targetType,
                                        t.associatedGenes = $associatedGenes,
                                        t.bucket = $bucket
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = target.UniId,
                                 targetId = target.TargetId,
                                 name = target.Name,
                                 targetType = target.TargetType,
                                 associatedGenes = target.GeneAccessionNumbers,
                                 bucket = target.Bucket
                             }).ExecuteAsync()
                             ;


                foreach (var accessionNumber in target.GeneAccessionNumbers)
                {
                    var query3 = @"
                        MATCH (g:Gene {accessionNumber: $_accessionNumber})
                        MATCH (t:Target {uniId: $uniId})
                        MERGE (t)-[:TARGETS {targetType: $targetType }]->(g)
                    ";
                    var (query3Results, _) = await _driver
                             .ExecutableQuery(query3).WithParameters(new
                             {
                                 _accessionNumber = accessionNumber,
                                 uniId = target.TargetId,
                                 targetType = target.TargetType
                             }).ExecuteAsync()
                             ;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddTarget");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Adding Target To Graph", ex);
            }
        }

        public async Task UpdateTarget(Target target)
        {
            _logger.LogInformation("UpdateTarget(): Updating target with id {TargetId}", target.TargetId);

            try
            {
                var query = @"
                    MATCH (t:Target {uniId: $uniId})
                    SET t.targetType = $targetType, t.bucket = $bucket
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = target.TargetId,
                                 targetType = target.TargetType,
                                 bucket = target.Bucket
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateTarget");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Updating Target In Graph", ex);
            }
        }

        public async Task UpdateAssociatedGenesOfTarget(Target target)
        {
            _logger.LogInformation("UpdateAssociatedGenesOfTargetInGraph(): Updating associated genes of target with id {TargetId}", target.TargetId);

            try
            {
                // Delete existing relations
                var query = @"
                    MATCH (t:Target {uniId: $uniId})-[r:TARGETS]->(g:Gene)
                            DELETE r
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 targetId = target.TargetId
                             }).ExecuteAsync()
                             ;
                // Set new associated genes
                var query2 = @"
                    MATCH (t:Target {uniId: $uniId})
                    SET t.associatedGenes = $associatedGenes
                ";
                var (query2Results, _) = await _driver
                             .ExecutableQuery(query2).WithParameters(new
                             {
                                 uniId = target.TargetId,
                                 associatedGenes = target.GeneAccessionNumbers
                             }).ExecuteAsync()
                             ;
                // Create new relations
                foreach (var accessionNumber in target.GeneAccessionNumbers)
                {
                    var query3 = @"
                        MATCH (g:Gene {accessionNumber: $_accessionNumber})
                        MATCH (t:Target {uniId: $uniId})
                        MERGE (t)-[:TARGETS {targetType: $targetType }]->(g)
                    ";
                    var (query3Results, _) = await _driver
                             .ExecutableQuery(query3).WithParameters(new
                             {
                                 _accessionNumber = accessionNumber,
                                 uniId = target.TargetId,
                                 targetType = target.TargetType
                             }).ExecuteAsync()
                             ;
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAssociatedGenesOfTargetInGraph");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Updating Associated Genes Of Target In Graph", ex);
            }
        }

        public async Task DeleteTarget(string targetId)
        {
            _logger.LogInformation("DeleteTarget(): Deleting target with id {targetId}", targetId);

            try
            {
                var query = @"
                     MATCH (t:Target {uniId: $uniId})
                        DETACH DELETE t
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = targetId,
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteTarget");
                throw new RepositoryException(nameof(GraphRepositoryForTarget), "Error Deleting Target From Graph", ex);
            }
        }

        public async Task RenameTarget(string targetId, string newName)
        {
            _logger.LogInformation("RenameTarget(): Renaming target with id {targetId} to {NewName}", targetId, newName);

            try
            {
                var query = @"
                     MATCH (t:Target {uniId: $uniId})
                            SET t.name = $_newName
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = targetId,
                                 _newName = newName
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Rename target");
                throw new RepositoryException(nameof(GraphRepositoryForScreen), "Error Renaming Target In Graph", ex);
            }
        }
    }
}
