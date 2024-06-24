
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Screens;
using Microsoft.Extensions.Logging;


namespace Horizon.Infrastructure.Repositories
{
    public partial class GraphRepositoryForHitCollection : IGraphRepositoryForHitCollection
    {

        public async Task AddHit(Hit hit)
        {
            _logger.LogInformation("AddHit(): Adding hit with id {HitId} and MoleculeId {MoleculeId} and hitCollectionId {hitCollectionId}", hit.HitId, hit.MoleculeId, hit.HitCollectionId);

            try
            {
                if (!string.IsNullOrEmpty(hit.MoleculeId))
                {
                    var mergeMoleculeQuery = @"
                        MERGE (m:Molecule {uniId: $moleculeId})
                        WITH m
                        MATCH (hc:HitCollection {uniId: $hitCollectionId})
                        MERGE (hc)-[:HIT_MOLECULE {hitId: $hitId, library: $library, requestedSMILES: $requestedSMILES}]->(m)
                    ";

                    var (queryResults2, _) = await _driver
                             .ExecutableQuery(mergeMoleculeQuery).WithParameters(new
                             {
                                 hitId = hit.HitId,
                                 moleculeId = hit.MoleculeId,
                                 hitCollectionId = hit.HitCollectionId,
                                 library = hit.Library,
                                 requestedSMILES = hit.RequestedSMILES
                             }).ExecuteAsync()
                             ;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Adding Hit To Graph", ex);
            }
        }

        public async Task UpdateHit(Hit hit)
        {
            _logger.LogInformation("UpdateHit(): Updating hit with id {HitId} and hitCollectionId {hitCollectionId}", hit.HitId, hit.HitCollectionId);
            try
            {
                var query = @"
                    MATCH (hc:HitCollection {uniId: $hitCollectionId})- [r:HIT_MOLECULE]->(m:Molecule)
                    WHERE r.hitId = $hitId
                    SET 
                        r.library = $library
        ";

                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        hitId = hit.HitId,
                        hitCollectionId = hit.HitCollectionId,
                        library = hit.Library,
                    }).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Updating Hit In Graph", ex);
            }
        }


        public async Task DeleteHit(string hitId)
        {
            _logger.LogInformation("DeleteHit(): Deleting hit with id {HitId}", hitId);

            try
            {
                var deleteHitQuery = @"
                    MATCH (hc:HitCollection)-[r:HIT_MOLECULE]->(m:Molecule)
                    WHERE r.hitId = $_hitId
                    DELETE r
        ";

                var (queryResults, _) = await _driver
                             .ExecutableQuery(deleteHitQuery).WithParameters(new
                             {
                                 _hitId = hitId,
                             }).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Deleting Hit From Graph", ex);
            }
        }

    }
}
