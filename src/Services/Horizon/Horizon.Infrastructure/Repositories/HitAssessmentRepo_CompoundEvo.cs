
using CQRS.Core.Exceptions;
using Horizon.Domain.HitAssessment;
using Microsoft.Extensions.Logging;

namespace Horizon.Infrastructure.Repositories
{
    public partial class HitAssessmentRepo
    {
        public async Task AddHaCEvo(HACompoundEvolution hACompoundEvolution)
        {
            _logger.LogInformation("AddHaCEvo(): Adding HA Compound Evolution with Ha Id {haId} and MoleculeId {MoleculeId}, CompoundEvolutionId {evoId}", hACompoundEvolution.HitAssessmentId, hACompoundEvolution.MoleculeId, hACompoundEvolution.CompoundEvolutionId);

            try
            {
                if (
                    !string.IsNullOrEmpty(hACompoundEvolution.MoleculeId) &&
                    !string.IsNullOrEmpty(hACompoundEvolution.CompoundEvolutionId
                    ))
                {
                    var mergeMoleculeQuery = @"
                        MERGE (m:Molecule {uniId: $moleculeId})
                        WITH m
                        MATCH (ha:HitAssessment {uniId: $hitAssessmentId})
                        MERGE (ha)-[:COMPOUND_EVO_MOLECULE {compoundEvolutionId: $compoundEvoId, stage: $stage}]->(m)
                    ";

                    var (queryResults2, _) = await _driver
                             .ExecutableQuery(mergeMoleculeQuery).WithParameters(new
                             {
                                 hitAssessmentId = hACompoundEvolution.HitAssessmentId,
                                 moleculeId = hACompoundEvolution.MoleculeId,
                                 stage = hACompoundEvolution.Stage ?? "HA",
                                 compoundEvoId = hACompoundEvolution.CompoundEvolutionId

                             }).ExecuteAsync()
                             ;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddHaCEvo");
                throw new RepositoryException(nameof(HitAssessmentRepo), "Error Adding Hit Assessment Compound Evolution To Graph", ex);
            }
        }

        public async Task UpdateHaCEvo(HACompoundEvolution hACompoundEvolution)
        {
            _logger.LogInformation("UpdateHaCEvo(): Updating HA Compound Evolution with Ha Id {haId}, CompoundEvolutionId {evoId}", hACompoundEvolution.HitAssessmentId, hACompoundEvolution.CompoundEvolutionId);
            try
            {
                if (
                    !string.IsNullOrEmpty(hACompoundEvolution.CompoundEvolutionId
                    ))
                {
                    var query = @"
                    MATCH (ha:HitAssessment {uniId: $hitAssessmentId}) - [r:COMPOUND_EVO_MOLECULE]->(m:Molecule)
                    WHERE r.compoundEvolutionId = $compoundEvoId
                    SET 
                        r.stage = $stage
        ";

                    var (queryResults, _) = await _driver
                        .ExecutableQuery(query).WithParameters(new
                        {
                            hitAssessmentId = hACompoundEvolution.HitAssessmentId,
                            compoundEvoId = hACompoundEvolution.CompoundEvolutionId,
                            stage = hACompoundEvolution.Stage ?? "HA"
                        }).ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHit");
                throw new RepositoryException(nameof(HitAssessmentRepo), "Error Updating Hit Assessment Compound Evolution In Graph", ex);
            }
        }


        public async Task DeleteHaCEvo(string compoundEvoId)
        {
            _logger.LogInformation("DeleteHaCEvo(): Deleting Hit Assessment Compound Evo with id {id}", compoundEvoId);

            if (string.IsNullOrEmpty(compoundEvoId))
            {
                _logger.LogError("DeleteHaCEvo(): compoundEvoId is null");
                throw new ArgumentNullException(nameof(compoundEvoId));
            }
            try
            {
                var query = @"
                    MATCH (ha:HitAssessment)-[r:COMPOUND_EVO_MOLECULE]->(m:Molecule)
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
                _logger.LogError(ex, "Error in DeleteHaCEvo()");
                throw new RepositoryException(nameof(HitAssessmentRepo), "Error Deleting Hit Assessment Compound Evolution From Graph", ex);
            }
        }
    }
}