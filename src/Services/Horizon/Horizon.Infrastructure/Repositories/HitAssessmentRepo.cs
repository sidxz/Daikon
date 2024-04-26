using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.HitAssessment;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Infrastructure.Repositories
{
    public class HitAssessmentRepo : IHitAssessmentRepo
    {

        private readonly IDriver _driver;
        private ILogger<HitAssessmentRepo> _logger;
        private readonly QueryConfig _queryConfig;

        public HitAssessmentRepo(IDriver driver, ILogger<HitAssessmentRepo> logger)
        {
            _driver = driver;
            _logger = logger;
            _queryConfig = new QueryConfig(database: Environment.GetEnvironmentVariable("Database") ?? "Horizon");
        }

        public async Task CreateIndexesAsync()
        {
            try
            {
                // var query = @"
                //   CREATE INDEX ha_uniId_index IF NOT EXISTS FOR (ha:HitAssessment) ON (ha.uniId);
                // ";
                // var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateIndexesAsync");
                throw new RepositoryException(nameof(HitAssessmentRepo), "Error Creating Indexes In Graph", ex);
            }
        }


        public async Task CreateConstraintsAsync()
        {
            try
            {
                var query = @"
                    CREATE CONSTRAINT ha_uniId_unique IF NOT EXISTS FOR (ha:HitAssessment) REQUIRE ha.uniId IS UNIQUE;
                ";
                var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateConstraintsAsync");
                throw new RepositoryException(nameof(HitAssessmentRepo), "Error Creating Constraints In Graph", ex);
            }
        }


        public async Task Create(HitAssessment hitAssessment)
        {
            try
            {
                _logger.LogInformation("Creating HitAssessment with UniId: {UniId}", hitAssessment.UniId);
                var query = @"
                MERGE (ha:HitAssessment { uniId: $uniId})
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
                    dateCreated = hitAssessment.DateCreated,
                };

                var (queryResults, _) = await _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();

                // Now set the relationship with the HitCollection

                if (!string.IsNullOrEmpty(hitAssessment.HitCollectionId))
                {
                    var hcRelQuery = @"
                    MATCH (ha:HitAssessment { uniId: $uniId})
                    MATCH (hc:HitCollection { uniId: $hitCollectionId})
                    MERGE (ha)-[:HIT_ASSESSMENT_OF]->(hc)
                    ";

                    var hcRelParameters = new
                    {
                        uniId = hitAssessment.UniId,
                        hitCollectionId = hitAssessment.HitCollectionId
                    };

                    var (hcRelQueryResults, _) = await _driver.ExecutableQuery(hcRelQuery).WithParameters(hcRelParameters).ExecuteAsync();
                }


                // Now set the relationship with the Primary Molecule
                var pmRelQuery = @"
                    MATCH (ha:HitAssessment { uniId: $uniId})
                    MATCH (m:Molecule { uniId: $primaryMoleculeId})
                    MERGE (ha)-[:PRIMARY_MOLECULE ]->(m)
                    ";
                var pmRelParameters = new
                {
                    uniId = hitAssessment.UniId,
                    primaryMoleculeId = hitAssessment.PrimaryMoleculeId
                };

                var (pmRelQueryResults, _) = await _driver.ExecutableQuery(pmRelQuery).WithParameters(pmRelParameters).ExecuteAsync();

                // Now set the relationship with the Associated Molecules
                foreach (var molecule in hitAssessment.AssociatedMoleculeIds)
                {
                    var amRelQuery = @"
                    MATCH (ha:HitAssessment { uniId: $uniId})
                    MATCH (m:Molecule { uniId: $moleculeId})
                    MERGE (ha)-[:ASSOCIATED_MOLECULE ]->(m)
                    ";
                    var amRelParameters = new
                    {
                        uniId = hitAssessment.UniId,
                        moleculeId = molecule
                    };

                    var (amRelQueryResults, _) = await _driver.ExecutableQuery(amRelQuery).WithParameters(amRelParameters).ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create");
                throw new RepositoryException(nameof(HitAssessmentRepo), "Error Creating HitAssessment In Graph", ex);
            }

        }
        public Task Delete(string hitAssessmentId)
        {
            var query = @"
                MATCH (ha:HitAssessment { uniId: $uniId})
                DETACH DELETE ha
            ";
            var parameters = new
            {
                uniId = hitAssessmentId
            };

            return _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();
        }

        public Task Update(HitAssessment hitAssessment)
        {
            _logger.LogInformation("Updating HitAssessment with UniId: {UniId}", hitAssessment.UniId);
            // var jsonVal = System.Text.Json.JsonSerializer.Serialize(hitAssessment);
            // _logger.LogInformation("Updating HitAssessment with UniId: {UniId} {jsonVal}", hitAssessment.UniId, jsonVal);
            try
            {
                var query = @"
                MATCH (ha:HitAssessment { uniId: $uniId})
                SET 
                    ha.name = $name, 
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
                    name = hitAssessment.Name,
                    status = hitAssessment.Status,
                    orgId = hitAssessment.OrgId,
                    isHAComplete = hitAssessment.IsHAComplete,
                    isHASuccess = hitAssessment.IsHASuccess,
                    dateModified = hitAssessment.DateModified,
                    isModified = hitAssessment.IsModified
                };

                return _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update");
                throw new RepositoryException(nameof(HitAssessmentRepo), "Error Updating HitAssessment In Graph", ex);
            }


        }
    }
}