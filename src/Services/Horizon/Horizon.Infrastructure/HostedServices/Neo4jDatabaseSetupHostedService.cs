using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistence;
using Horizon.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Horizon.Infrastructure.HostedServices
{
    public class Neo4jDatabaseSetupHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Neo4jDatabaseSetupHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var geneGraphRepository = scope.ServiceProvider.GetRequiredService<IGraphRepositoryForGene>();
                await geneGraphRepository.CreateIndexesAsync();
                await geneGraphRepository.CreateConstraintsAsync();

                var targetGraphRepository = scope.ServiceProvider.GetRequiredService<IGraphRepositoryForTarget>();
                await targetGraphRepository.CreateIndexesAsync();
                await targetGraphRepository.CreateConstraintsAsync();

                var screenGraphRepository = scope.ServiceProvider.GetRequiredService<IGraphRepositoryForScreen>();
                await screenGraphRepository.CreateIndexesAsync();
                await screenGraphRepository.CreateConstraintsAsync();

                var hitCollectionGraphRepository = scope.ServiceProvider.GetRequiredService<IGraphRepositoryForHitCollection>();
                await hitCollectionGraphRepository.CreateIndexesAsync();
                await hitCollectionGraphRepository.CreateConstraintsAsync();

                var hitAssessmentGraphRepository = scope.ServiceProvider.GetRequiredService<IHitAssessmentRepo>();
                await hitAssessmentGraphRepository.CreateIndexesAsync();
                await hitAssessmentGraphRepository.CreateConstraintsAsync();

                var projectGraphRepository = scope.ServiceProvider.GetRequiredService<IProjectRepo>();
                await projectGraphRepository.CreateIndexesAsync();
                await projectGraphRepository.CreateConstraintsAsync();

                var mLogixGraphRepository = scope.ServiceProvider.GetRequiredService<IGraphRepositoryForMLogix>();
                await mLogixGraphRepository.CreateIndexesAsync();
                await mLogixGraphRepository.CreateConstraintsAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
