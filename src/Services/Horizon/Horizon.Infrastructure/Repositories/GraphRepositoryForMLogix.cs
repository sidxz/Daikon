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

        public GraphRepositoryForMLogix(IDriver driver, ILogger<GraphRepositoryForMLogix> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        public async Task CreateIndexesAsync()
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {

                    // Create the index if it does not exist
                    var createIndexQuery = "CREATE INDEX molecules_registration_id_index IF NOT EXISTS FOR (m:Molecules) ON (m.registrationId);";
                    await tx.RunAsync(createIndexQuery);
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateConstraintsAsync()
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    var createConstraintQuery = "CREATE CONSTRAINT molecules_uniId_constraint IF NOT EXISTS FOR (m:Molecules) REQUIRE m.uniId IS UNIQUE;";
                    await tx.RunAsync(createConstraintQuery);
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }


        public async Task AddMolecule(Molecule molecule)
        {
            _logger.LogInformation("Adding Molecule to Graph with RegistrationId: {RegistrationId}", molecule.RegistrationId);
            var session = _driver.AsyncSession();
            try
            {
                var retryPolicy = CreateRetryPolicy(_logger);

                await retryPolicy.ExecuteAsync(async () =>
                {
                    await session.ExecuteWriteAsync(async tx =>
                    {
                        var createOrUpdateMoleculeQuery = @"
                            MERGE (m:Molecule { registrationId: $registrationId })
                            ON CREATE SET
                                        m.uniId = $uniId,
                                        m.mLogixId = $mLogixId,
                                        m.name = $name,
                                        m.requestedSMILES = $requestedSMILES,
                                        m.smilesCanonical = $smilesCanonical
                            ON MATCH SET  
                                        m.uniId = $uniId,
                                        m.mLogixId = $mLogixId,
                                        m.name = $name,
                                        m.requestedSMILES = $requestedSMILES,
                                        m.smilesCanonical = $smilesCanonical
";

                        await tx.RunAsync(createOrUpdateMoleculeQuery, new
                        {
                            uniId = molecule.UniId,
                            mLogixId = molecule.MLogixId,
                            registrationId = molecule.RegistrationId,
                            name = molecule.Name,
                            requestedSMILES = molecule.RequestedSMILES,
                            smilesCanonical = molecule.SmilesCanonical,
                        });

                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddMolecule");
                throw new RepositoryException(nameof(GraphRepositoryForMLogix), "Error Adding Molecule To Graph", ex);
            }
            finally
            {
                await session.CloseAsync();
            }
        }




        // Define the RetryPolicy
        /*
        This method call specifies the type of exception that the policy should handle, which in this case is ClientException.
        The lambda expression ex => ex.Message.Contains("ConstraintValidationFailed") further filters these exceptions to only 
        those where the exception's message contains the text "ConstraintValidationFailed". 
        This is likely a specific error message you expect from Neo4j when a unique constraint is violated.

        The need for this retry policy is because multiple nodes of same functional category were created in the graph database
        when uploading in bulk.
        */
        private static readonly Func<ILogger<GraphRepositoryForMLogix>, IAsyncPolicy> CreateRetryPolicy = logger => Policy
             .Handle<TransientException>()
                .Or<ServiceUnavailableException>()
            .WaitAndRetryAsync(
                new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(7)
                },
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning("Attempt {RetryCount} failed with exception. Waiting {TimeSpan} before next retry. Exception: {Exception}",
                        retryCount, timeSpan, exception.Message);
                }
            );
    }
}