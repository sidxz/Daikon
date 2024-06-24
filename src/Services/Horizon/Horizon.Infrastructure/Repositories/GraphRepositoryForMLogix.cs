using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.MLogix;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Polly;

namespace Horizon.Infrastructure.Repositories
{
    public class GraphRepositoryForMLogix : IGraphRepositoryForMLogix
    {
        private readonly IDriver _driver;
        private ILogger<GraphRepositoryForMLogix> _logger;
        private readonly QueryConfig _queryConfig;

        public GraphRepositoryForMLogix(IDriver driver, ILogger<GraphRepositoryForMLogix> logger)
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
                //   CREATE INDEX molecules_uniId_index IF NOT EXISTS FOR (m:Molecules) ON (m.uniId);
                // ";
                // var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();

                var query2 = @"
                  CREATE INDEX molecules_registration_id_index IF NOT EXISTS FOR (m:Molecules) ON (m.registrationId);
                ";
                var (query2Results, _) = await _driver.ExecutableQuery(query2).ExecuteAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateIndexesAsync");
                throw new RepositoryException(nameof(GraphRepositoryForMLogix), "Error Creating Indexes In Graph", ex);
            }
        }

        public async Task CreateConstraintsAsync()
        {
            try
            {
                var query = @"
                    CREATE CONSTRAINT molecule_uniId_unique IF NOT EXISTS FOR (m:Molecules) REQUIRE m.uniId IS UNIQUE;
                ";
                var (queryResults, _) = await _driver.ExecutableQuery(query).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateConstraintsAsync");
                throw new RepositoryException(nameof(GraphRepositoryForMLogix), "Error Creating Constraints In Graph", ex);
            }
        }
        public async Task AddMolecule(Molecule molecule)
        {
            _logger.LogInformation("Adding Molecule to Graph with RegistrationId: {RegistrationId}", molecule.RegistrationId);

            var mergeMolQuery = @"
                     MERGE (m:Molecule { uniId: $uniId })
                            ON CREATE SET
                                        m.registrationId = $registrationId,
                                        m.mLogixId = $mLogixId,
                                        m.name = $name,
                                        m.requestedSMILES = $requestedSMILES,
                                        m.smilesCanonical = $smilesCanonical
                            ON MATCH SET  
                                        m.registrationId = $registrationId,
                                        m.mLogixId = $mLogixId,
                                        m.name = $name,
                                        m.requestedSMILES = $requestedSMILES,
                                        m.smilesCanonical = $smilesCanonical
                ";

            var (queryResults, _) = await _driver
                         .ExecutableQuery(mergeMolQuery).WithParameters(new
                         {
                             uniId = molecule.UniId,
                             mLogixId = molecule.MLogixId,
                             registrationId = molecule.RegistrationId,
                             name = molecule.Name,
                             requestedSMILES = molecule.RequestedSMILES,
                             smilesCanonical = molecule.SmilesCanonical,
                         }).ExecuteAsync()
                         ;
        }

        public async Task UpdateMolecule(Molecule molecule)
        {
            _logger.LogInformation("Updating Molecule in Graph with UniId: {UniId}", molecule.UniId);

            var mergeMolQuery = @"
                     MERGE (m:Molecule { uniId: $uniId })
                            ON CREATE SET
                                        m.registrationId = $registrationId,
                                        m.mLogixId = $mLogixId,
                                        m.name = $name,
                                        m.requestedSMILES = $requestedSMILES,
                                        m.smilesCanonical = $smilesCanonical
                            ON MATCH SET
                                        m.registrationId = $registrationId,
                                        m.mLogixId = $mLogixId,
                                        m.name = $name,
                                        m.requestedSMILES = $requestedSMILES,
                                        m.smilesCanonical = $smilesCanonical
                ";

            var (queryResults, _) = await _driver
                         .ExecutableQuery(mergeMolQuery).WithParameters(new
                         {
                             uniId = molecule.UniId,
                             mLogixId = molecule.MLogixId,
                             registrationId = molecule.RegistrationId,
                             name = molecule.Name,
                             requestedSMILES = molecule.RequestedSMILES,
                             smilesCanonical = molecule.SmilesCanonical,
                         }).ExecuteAsync()
                         ;
        }
    }
}