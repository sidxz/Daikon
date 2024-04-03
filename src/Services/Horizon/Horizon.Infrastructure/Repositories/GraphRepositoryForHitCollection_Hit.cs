
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
            _logger.LogInformation("AddHit(): Adding hit with id {HitId} and MoleculeId {MoleculeId} MoleculeRegId {MoleculeRegId} and hitCollectionId {hitCollectionId}", hit.HitId, hit.MoleculeId, hit.MoleculeRegistrationId, hit.HitCollectionId);

            try
            {
                var query = @"
                    MATCH (hc:HitCollection {uniId: $hitCollectionId})
                    MERGE (hc)-[:HIT]->(hit:Hit {uniId: $hitId})
                        ON CREATE SET 
                            hit.library = $library, 
                            hit.requestedSMILES = $requestedSMILES
                        ON MATCH SET
                            hit.library = $library, 
                            hit.requestedSMILES = $requestedSMILES
                ";

                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 hitCollectionId = hit.HitCollectionId,
                                 hitId = hit.HitId,
                                 library = hit.Library,
                                 requestedSMILES = hit.RequestedSMILES,
                             }).ExecuteAsync()
                             ;

                if (!string.IsNullOrEmpty(hit.MoleculeRegistrationId))
                {
                    var mergeMoleculeQuery = @"
                        MERGE (m:Molecule {registrationId: $moleculeRegistrationId})
                        WITH m
                        MATCH (hit:Hit {uniId: $hitId})
                        MERGE (hit)-[:HIT_MOLECULE]->(m)
                    ";

                    var (queryResults2, _) = await _driver
                             .ExecutableQuery(mergeMoleculeQuery).WithParameters(new
                             {
                                 hitId = hit.HitId,
                                 moleculeRegistrationId = hit.MoleculeRegistrationId,
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
            _logger.LogInformation("UpdateHit(): Updating hit with id {HitId} and MoleculeId {MoleculeID} and hitCollectionId {hitCollectionId}", hit.HitId, hit.MoleculeId, hit.HitCollectionId);

            try
            {
                var query = @"
                    MATCH (hit:Hit {uniId: $hitId})
                        SET 
                            hit.requestedSMILES = $requestedSMILES,
                            hit.library = $library
                ";

                var (queryResults, _) = await _driver
                    .ExecutableQuery(query).WithParameters(new
                    {
                        uniId = hit.HitId,
                        requestedSMILES = hit.RequestedSMILES,
                        library = hit.Library
                    }).ExecuteAsync()
                    ;
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
                     MATCH (hit:Hit {uniId: $_hitId})
                    DETACH DELETE hit
                ";

                var (queryResults, _) = await _driver
                             .ExecutableQuery(deleteHitQuery).WithParameters(new
                             {
                                 _hitId = hitId,
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteHit");
                throw new RepositoryException(nameof(GraphRepositoryForHitCollection), "Error Deleting Hit From Graph", ex);
            }
        }
    }
}
