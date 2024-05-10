
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Projects;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Infrastructure.Repositories
{
    public class ProjectRepo : IProjectRepo
    {

        private readonly IDriver _driver;
        private ILogger<ProjectRepo> _logger;
        private readonly QueryConfig _queryConfig;

        public ProjectRepo(IDriver driver, ILogger<ProjectRepo> logger)
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
                //   CREATE INDEX project_uniId_index IF NOT EXISTS FOR (p:Project) ON (p.uniId);
                // ";
                // var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateIndexesAsync");
                throw new RepositoryException(nameof(ProjectRepo), "Error Creating Indexes In Graph", ex);
            }
        }


        public async Task CreateConstraintsAsync()
        {
            try
            {
                var query = @"
                    CREATE CONSTRAINT project_uniId_unique IF NOT EXISTS FOR (p:Project) REQUIRE p.uniId IS UNIQUE;
                ";
                var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateConstraintsAsync");
                throw new RepositoryException(nameof(ProjectRepo), "Error Creating Constraints In Graph", ex);
            }
        }


        public async Task Create(Project project)
        {
            try
            {
                _logger.LogInformation("Creating Project with UniId: {UniId}", project.UniId);
                var query = @"
                MERGE (p:Project { uniId: $uniId})
                ON CREATE SET 
                    p.name = $name, 
                    p.status = $status,
                    p.stage = $stage,
                    p.orgId = $orgId, 
                    p.isProjectComplete = $isProjectComplete, 
                    p.isProjectRemoved = $isProjectRemoved, 
                    p.dateCreated = $dateCreated
                ON MATCH SET 
                    p.name = $name, 
                    p.status = $status,
                    p.stage = $stage,
                    p.orgId = $orgId, 
                    p.isProjectComplete = $isProjectComplete, 
                    p.isProjectRemoved = $isProjectRemoved, 
                    p.dateCreated = $dateCreated
            ";
                var parameters = new
                {
                    uniId = project.ProjectId,
                    name = project.Name,
                    status = project.Status,
                    stage = project.Stage,
                    orgId = project.OrgId,
                    isProjectComplete = project.IsProjectComplete,
                    isProjectRemoved = project.IsProjectRemoved,
                    dateCreated = project.DateCreated,
                };

                var (queryResults, _) = await _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();

                // Now set the relationship with the HitAssessment

                if (!string.IsNullOrEmpty(project.HitAssessmentId))
                {
                    var hcRelQuery = @"
                    MATCH (p:Project { uniId: $uniId})
                    MATCH (ha:HitAssessment { uniId: $hitAssessmentId})
                    MERGE (p)-[:PROJECT_OF]->(ha)
                    ";

                    var hcRelParameters = new
                    {
                        uniId = project.ProjectId,
                        hitAssessmentId = project.HitAssessmentId
                    };

                    var (hcRelQueryResults, _) = await _driver.ExecutableQuery(hcRelQuery).WithParameters(hcRelParameters).ExecuteAsync();
                }


                // Now set the relationship with the Primary Molecule
                if (!string.IsNullOrEmpty(project.PrimaryMoleculeId))
                {
                    var pmRelQuery = @"
                    MATCH (p:Project { uniId: $uniId})
                    MATCH (m:Molecule { uniId: $primaryMoleculeId})
                    MERGE (p)-[:PRIMARY_MOLECULE ]->(m)
                    ";
                    var pmRelParameters = new
                    {
                        uniId = project.UniId,
                        primaryMoleculeId = project.PrimaryMoleculeId
                    };

                    var (pmRelQueryResults, _) = await _driver.ExecutableQuery(pmRelQuery).WithParameters(pmRelParameters).ExecuteAsync();
                }

                if (!string.IsNullOrEmpty(project.HitMoleculeId))
                {
                    var amRelQuery = @"
                    MATCH (p:Project { uniId: $uniId})
                    MATCH (m:Molecule { uniId: $hitMoleculeId})
                    MERGE (p)-[:INITIAL_HIT_MOLECULE ]->(m)
                    ";
                    var amRelParameters = new
                    {
                        uniId = project.UniId,
                        hitMoleculeId = project.HitMoleculeId
                    };

                    var (amRelQueryResults, _) = await _driver.ExecutableQuery(amRelQuery).WithParameters(amRelParameters).ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create");
                throw new RepositoryException(nameof(ProjectRepo), "Error Creating Project In Graph", ex);
            }

        }
        public Task Delete(string hitAssessmentId)
        {
            var query = @"
                MATCH (p:Project { uniId: $uniId})
                DETACH DELETE p
            ";
            var parameters = new
            {
                uniId = hitAssessmentId
            };

            return _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();
        }

        public Task Update(Project project)
        {
            try
            {
                _logger.LogInformation("Updating Project with UniId: {UniId}", project.UniId);
                var json = System.Text.Json.JsonSerializer.Serialize(project);
                _logger.LogInformation(json);

                var query = @"
                MATCH (p:Project { uniId: $uniId})
                SET 
                    p.status = $status,
                    p.stage = $stage,
                    p.orgId = $orgId, 
                    p.isProjectComplete = $isProjectComplete, 
                    p.isProjectRemoved = $isProjectRemoved, 
                    p.dateCreated = $dateCreated,
                    p.isModified = $isModified,
                    p.dateModified = $dateModified
            ";
                var parameters = new
                {
                    uniId = project.ProjectId,
                    status = project.Status ?? "",
                    stage = project.Stage ?? "",
                    orgId = project.OrgId ?? "",
                    isProjectComplete = project.IsProjectComplete,
                    isProjectRemoved = project.IsProjectRemoved,
                    dateCreated = project.DateCreated ?? DateTime.UtcNow,
                    isModified = project.IsModified,
                    dateModified = project.DateModified ?? DateTime.UtcNow
                };

                return _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update");
                throw new RepositoryException(nameof(ProjectRepo), "Error Updating Project In Graph", ex);
            }
        }

        public async Task Rename(Project project)
        {
            try
            {
                _logger.LogInformation("Renaming Project with UniId: {UniId}", project.UniId);
                var query = @"
                MATCH (p:Project { uniId: $uniId})
                SET 
                    p.name = $name,
                    p.isModified = $isModified,
                    p.dateModified = $dateModified
            ";
                var parameters = new
                {
                    uniId = project.ProjectId,
                    name = project.Name,
                    isModified = project.IsModified,
                    dateModified = project.DateModified
                };

                var (queryResults, _) = await _driver.ExecutableQuery(query).WithParameters(parameters).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Rename");
                throw new RepositoryException(nameof(ProjectRepo), "Error Renaming Project In Graph", ex);
            }
        }

        public async Task UpdateHitAssessmentAssociation(Project project)
        {
            try
            {
                _logger.LogInformation("UpdateHitAssessmentAssociation with UniId: {UniId}", project.UniId);
                // delete previous associations
                var deleteHaQuery = @"
                MATCH (p:Project { uniId: $uniId})-[r:PROJECT_OF]->(:HitAssessment)
                DELETE r
            ";
                var deleteHaParameters = new
                {
                    uniId = project.UniId,
                };

                var (deleteHaQueryResults, _) = await _driver.ExecutableQuery(deleteHaQuery).WithParameters(deleteHaParameters).ExecuteAsync();

                // Now set the relationship with the HitAssessment
                if (!string.IsNullOrEmpty(project.HitAssessmentId))
                {
                    var hcRelQuery = @"
                    MATCH (p:Project { uniId: $uniId})
                    MATCH (ha:HitAssessment { uniId: $hitAssessmentId})
                    MERGE (p)-[:PROJECT_OF]->(ha)
                    ";

                    var hcRelParameters = new
                    {
                        uniId = project.UniId,
                        hitAssessmentId = project.HitAssessmentId
                    };

                    var (hcRelQueryResults, _) = await _driver.ExecutableQuery(hcRelQuery).WithParameters(hcRelParameters).ExecuteAsync();
                }


                // Now delete the relationship with the Primary Molecule
                var deletePmRelQuery = @"
                MATCH (p:Project { uniId: $uniId})-[r:PRIMARY_MOLECULE]->(:Molecule)
                DELETE r
            ";
                var deletePmRelParameters = new
                {
                    uniId = project.UniId,
                };

                var (deletePmRelQueryResults, _) = await _driver.ExecutableQuery(deletePmRelQuery).WithParameters(deletePmRelParameters).ExecuteAsync();

                // Now set the relationship with the Primary Molecule
                if (!string.IsNullOrEmpty(project.PrimaryMoleculeId))
                {
                    var pmRelQuery = @"
                    MATCH (p:Project { uniId: $uniId})
                    MATCH (m:Molecule { uniId: $primaryMoleculeId})
                    MERGE (p)-[:PRIMARY_MOLECULE ]->(m)
                    ";
                    var pmRelParameters = new
                    {
                        uniId = project.UniId,
                        primaryMoleculeId = project.PrimaryMoleculeId
                    };

                    var (pmRelQueryResults, _) = await _driver.ExecutableQuery(pmRelQuery).WithParameters(pmRelParameters).ExecuteAsync();
                }

                // Now delete the relationship with the Hit Molecule
                var deleteAmRelQuery = @"
                MATCH (p:Project { uniId: $uniId})-[r:INITIAL_HIT_MOLECULE]->(:Molecule)
                DELETE r
            ";
                var deleteAmRelParameters = new
                {
                    uniId = project.UniId,
                };

                var (deleteAmRelQueryResults, _) = await _driver.ExecutableQuery(deleteAmRelQuery).WithParameters(deleteAmRelParameters).ExecuteAsync();

                // Now set the relationship with the Hit Molecule
                if (!string.IsNullOrEmpty(project.HitMoleculeId))
                {
                    var amRelQuery = @"
                    MATCH (p:Project { uniId: $uniId})
                    MATCH (m:Molecule { uniId: $hitMoleculeId})
                    MERGE (p)-[:INITIAL_HIT_MOLECULE ]->(m)
                    ";
                    var amRelParameters = new
                    {
                        uniId = project.UniId,
                        hitMoleculeId = project.HitMoleculeId
                    };

                    var (amRelQueryResults, _) = await _driver.ExecutableQuery(amRelQuery).WithParameters(amRelParameters).ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHitAssessmentAssociation");
                throw new RepositoryException(nameof(ProjectRepo), "Error Updating UpdateHitAssessmentAssociation In Graph", ex);
            }
        }
    }

}