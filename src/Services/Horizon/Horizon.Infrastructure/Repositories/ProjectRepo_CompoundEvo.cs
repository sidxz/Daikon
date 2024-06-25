
using CQRS.Core.Exceptions;
using Horizon.Domain.HitAssessment;
using Horizon.Domain.Projects;
using Microsoft.Extensions.Logging;

namespace Horizon.Infrastructure.Repositories
{
    public partial class ProjectRepo
    {
        public async Task AddProjectCEvo(ProjectCompoundEvolution compoundEvolution)
        {
            _logger.LogInformation("AddProjectCEvo(): Adding Project Compound Evolution with Project Id {pId} and MoleculeId {MoleculeId}, CompoundEvolutionId {evoId}", compoundEvolution.ProjectId, compoundEvolution.MoleculeId, compoundEvolution.CompoundEvolutionId);

            try
            {
                if (
                    !string.IsNullOrEmpty(compoundEvolution.MoleculeId) &&
                    !string.IsNullOrEmpty(compoundEvolution.CompoundEvolutionId
                    ))
                {
                    var mergeMoleculeQuery = @"
                        MERGE (m:Molecule {uniId: $moleculeId})
                        WITH m
                        MATCH (p:Project {uniId: $projectId})
                        MERGE (p)-[:COMPOUND_EVO_MOLECULE {compoundEvolutionId: $compoundEvoId, stage: $stage}]->(m)
                    ";

                    var (queryResults2, _) = await _driver
                             .ExecutableQuery(mergeMoleculeQuery).WithParameters(new
                             {
                                 projectId = compoundEvolution.ProjectId,
                                 moleculeId = compoundEvolution.MoleculeId,
                                 stage = compoundEvolution.Stage ?? "Unknown",
                                 compoundEvoId = compoundEvolution.CompoundEvolutionId

                             }).ExecuteAsync()
                             ;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddProjectCEvo");
                throw new RepositoryException(nameof(ProjectRepo), "Error Adding Project Assessment Compound Evolution To Graph", ex);
            }
        }

        public async Task UpdateProjectCEvo(ProjectCompoundEvolution compoundEvolution)
        {
            _logger.LogInformation("UpdateProjectCEvo(): Updating Project Compound Evolution with Project Id {pId}, CompoundEvolutionId {evoId}", compoundEvolution.ProjectId, compoundEvolution.CompoundEvolutionId);
            try
            {
                if (
                    !string.IsNullOrEmpty(compoundEvolution.CompoundEvolutionId
                    ))
                {
                    var query = @"
                    MATCH (p:Project {uniId: $projectId}) - [r:COMPOUND_EVO_MOLECULE]->(m:Molecule)
                    WHERE r.compoundEvolutionId = $compoundEvoId
                    SET 
                        r.stage = $stage
        ";

                    var (queryResults, _) = await _driver
                        .ExecutableQuery(query).WithParameters(new
                        {
                            projectId = compoundEvolution.ProjectId,
                            compoundEvoId = compoundEvolution.CompoundEvolutionId,
                            stage = compoundEvolution.Stage ?? "Unknown"
                        }).ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateProjectCEvo");
                throw new RepositoryException(nameof(ProjectRepo), "Error Updating Project Assessment Compound Evolution To Graph", ex);
            }
        }


        public async Task DeleteProjectCEvo(string compoundEvoId)
        {
            _logger.LogInformation("DeleteProjectCEvo(): Deleting Project Compound Evo with id {id}", compoundEvoId);

            if (string.IsNullOrEmpty(compoundEvoId))
            {
                _logger.LogError("DeleteProjectCEvo(): compoundEvoId is null");
                throw new ArgumentNullException(nameof(compoundEvoId));
            }
            try
            {
                var query = @"
                    MATCH (p:Project)-[r:COMPOUND_EVO_MOLECULE]->(m:Molecule)
                    WHERE r.compoundEvolutionId = $_compoundEvolutionId
                    DELETE r
        ";

                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 _compoundEvolutionId = compoundEvoId,
                             }).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteProjectCEvo");
                throw new RepositoryException(nameof(ProjectRepo), "Error Deleting Project Assessment Compound Evolution To Graph", ex);
            }
        }
    }
}