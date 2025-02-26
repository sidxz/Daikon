
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistence;
using Horizon.Domain.HitAssessment;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Infrastructure.Repositories
{
    public partial class HitAssessmentRepo : IHitAssessmentRepo
    {
        private readonly IDriver _driver;
        private readonly ILogger<HitAssessmentRepo> _logger;
        private readonly QueryConfig _queryConfig;

        public HitAssessmentRepo(IDriver driver, ILogger<HitAssessmentRepo> logger)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queryConfig = new QueryConfig(
                database: Environment.GetEnvironmentVariable("Database") ?? "Horizon"
            );
        }

        public async Task CreateIndexesAsync()
        {
            try
            {
                // Implement index creation logic when needed.
                _logger.LogInformation("CreateIndexesAsync invoked.");
            }
            catch (Exception ex)
            {
                LogAndThrowRepositoryException("Error in CreateIndexesAsync", ex);
            }
        }

        public async Task CreateConstraintsAsync()
        {
            try
            {
                const string query = @"
                    CREATE CONSTRAINT ha_uniId_unique IF NOT EXISTS 
                    FOR (ha:HitAssessment) 
                    REQUIRE ha.uniId IS UNIQUE;
                ";

                await ExecuteQueryAsync(query);
                _logger.LogInformation("Constraints created successfully.");
            }
            catch (Exception ex)
            {
                LogAndThrowRepositoryException("Error in CreateConstraintsAsync", ex);
            }
        }

        public async Task Create(HitAssessment hitAssessment)
        {
            ArgumentNullException.ThrowIfNull(hitAssessment);
            try
            {
                _logger.LogInformation("Creating HitAssessment with UniId: {UniId}", hitAssessment.UniId);

                // Create or update the HitAssessment node
                const string query = @"
                    MERGE (ha:HitAssessment { uniId: $uniId })
                    ON CREATE SET 
                        ha.name = $name, 
                        ha.status = $status, 
                        ha.orgId = $orgId, 
                        ha.isHAComplete = $isHAComplete, 
                        ha.isHASuccess = $isHASuccess, 
                        ha.dateCreated = $dateCreated
                    ON MATCH SET 
                        ha.name = $name, 
                        ha.status = $status, 
                        ha.orgId = $orgId, 
                        ha.isHAComplete = $isHAComplete, 
                        ha.isHASuccess = $isHASuccess, 
                        ha.dateCreated = $dateCreated
                ";

                var parameters = new
                {
                    uniId = hitAssessment.HitAssessmentId,
                    name = hitAssessment.Name,
                    status = hitAssessment.Status,
                    orgId = hitAssessment.OrgId,
                    isHAComplete = hitAssessment.IsHAComplete,
                    isHASuccess = hitAssessment.IsHASuccess,
                    dateCreated = hitAssessment.DateCreated
                };

                await ExecuteQueryAsync(query, parameters);

                // Create relationships
                await CreateRelationshipIfNotEmptyAsync(
                    hitAssessment.HitCollectionId,
                    hitAssessment.UniId,
                    "HIT_ASSESSMENT_OF",
                    "HitCollection"
                );

                await CreateRelationshipIfNotEmptyAsync(
                    hitAssessment.PrimaryMoleculeId,
                    hitAssessment.UniId,
                    "PRIMARY_MOLECULE",
                    "Molecule"
                );

                if (hitAssessment.AssociatedMoleculeIds != null)
                {
                    foreach (var moleculeId in hitAssessment.AssociatedMoleculeIds)
                    {
                        await CreateRelationshipIfNotEmptyAsync(
                            moleculeId,
                            hitAssessment.UniId,
                            "ASSOCIATED_MOLECULE",
                            "Molecule"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                LogAndThrowRepositoryException("Error in Create", ex);
            }
        }

        public async Task Update(HitAssessment hitAssessment)
        {
            if (hitAssessment == null) throw new ArgumentNullException(nameof(hitAssessment));

            try
            {
                _logger.LogInformation("Updating HitAssessment with UniId: {UniId}", hitAssessment.UniId);

                // Update HitAssessment node properties
                const string query = @"
                    MATCH (ha:HitAssessment { uniId: $uniId })
                    SET 
                        ha.status = $status, 
                        ha.orgId = $orgId, 
                        ha.isHAComplete = $isHAComplete, 
                        ha.isHASuccess = $isHASuccess, 
                        ha.dateModified = $dateModified,
                        ha.isModified = $isModified
                ";

                var parameters = new
                {
                    uniId = hitAssessment.HitAssessmentId,
                    status = hitAssessment.Status,
                    orgId = hitAssessment.OrgId,
                    isHAComplete = hitAssessment.IsHAComplete,
                    isHASuccess = hitAssessment.IsHASuccess,
                    dateModified = hitAssessment.DateModified,
                    isModified = hitAssessment.IsModified
                };

                await ExecuteQueryAsync(query, parameters);

                // Update relationships
                await UpdateRelationshipAsync(hitAssessment.UniId, hitAssessment.HitCollectionId, "HIT_ASSESSMENT_OF", "HitCollection");
                await UpdateRelationshipAsync(hitAssessment.UniId, hitAssessment.PrimaryMoleculeId, "PRIMARY_MOLECULE", "Molecule");

                if (hitAssessment.AssociatedMoleculeIds != null)
                {
                    await ClearRelationshipsAsync(hitAssessment.UniId, "ASSOCIATED_MOLECULE");
                    foreach (var moleculeId in hitAssessment.AssociatedMoleculeIds)
                    {
                        await CreateRelationshipIfNotEmptyAsync(moleculeId, hitAssessment.UniId, "ASSOCIATED_MOLECULE", "Molecule");
                    }
                }
            }
            catch (Exception ex)
            {
                LogAndThrowRepositoryException("Error in Update", ex);
            }
        }

        public async Task Rename(HitAssessment hitAssessment)
        {
            if (hitAssessment == null) throw new ArgumentNullException(nameof(hitAssessment));

            const string query = @"
                MATCH (ha:HitAssessment { uniId: $uniId })
                SET ha.name = $name
            ";

            var parameters = new
            {
                uniId = hitAssessment.UniId,
                name = hitAssessment.Name
            };

            await ExecuteQueryAsync(query, parameters);
        }

        public async Task Delete(string hitAssessmentId)
        {
            if (string.IsNullOrEmpty(hitAssessmentId))
                throw new ArgumentException("Invalid HitAssessmentId.", nameof(hitAssessmentId));

            const string query = @"
                MATCH (ha:HitAssessment { uniId: $uniId })
                DETACH DELETE ha
            ";

            var parameters = new { uniId = hitAssessmentId };

            await ExecuteQueryAsync(query, parameters);
        }

        private async Task ExecuteQueryAsync(string query, object parameters = null)
        {
            try
            {
                await _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {Query}", query);
                throw;
            }
        }

        private async Task CreateRelationshipIfNotEmptyAsync(string relatedId, string hitAssessmentId, string relationshipType, string relatedNodeLabel)
        {
            if (string.IsNullOrEmpty(relatedId)) return;

            var query = $@"
                MATCH (ha:HitAssessment {{ uniId: $hitAssessmentId }})
                MATCH ({relatedNodeLabel.ToLower()}:{relatedNodeLabel} {{ uniId: $relatedId }})
                MERGE (ha)-[:{relationshipType}]->({relatedNodeLabel.ToLower()})
            ";

            var parameters = new { hitAssessmentId, relatedId };
            await ExecuteQueryAsync(query, parameters);
        }

        private async Task UpdateRelationshipAsync(string hitAssessmentId, string relatedId, string relationshipType, string relatedNodeLabel)
        {
            await ClearRelationshipsAsync(hitAssessmentId, relationshipType);
            await CreateRelationshipIfNotEmptyAsync(relatedId, hitAssessmentId, relationshipType, relatedNodeLabel);
        }

        private async Task ClearRelationshipsAsync(string hitAssessmentId, string relationshipType)
        {
            var query = $@"
                MATCH (ha:HitAssessment {{ uniId: $hitAssessmentId }})
                OPTIONAL MATCH (ha)-[r:{relationshipType}]->()
                DELETE r
            ";

            var parameters = new { hitAssessmentId };
            await ExecuteQueryAsync(query, parameters);
        }

        private void LogAndThrowRepositoryException(string message, Exception ex)
        {
            _logger.LogError(ex, message);
            throw new RepositoryException(nameof(HitAssessmentRepo), message, ex);
        }
    }
}
