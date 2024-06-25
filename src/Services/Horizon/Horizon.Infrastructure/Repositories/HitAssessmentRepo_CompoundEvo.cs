
using CQRS.Core.Exceptions;
using Horizon.Domain.HitAssessment;
using Microsoft.Extensions.Logging;

namespace Horizon.Infrastructure.Repositories
{
    public partial class HitAssessmentRepo
    {
        public async Task AddHaCEvo(HACompoundEvolution hACompoundEvolution)
        {
            _logger.LogInformation("AddHaCEvo(): Adding HA Compound Evolution with Ha Id {haId} and MoleculeId {MoleculeId}", hACompoundEvolution.HitAssessmentId, hACompoundEvolution.MoleculeId);

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
                _logger.LogError(ex, "Error in AddHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Adding Hit Assessment Compound Evolution To Graph", ex);
            }
        }
    }
}