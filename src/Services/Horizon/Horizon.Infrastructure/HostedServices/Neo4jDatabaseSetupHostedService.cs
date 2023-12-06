using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistance;
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
                var graphRepository = scope.ServiceProvider.GetRequiredService<IGraphRepository>();
                await graphRepository.CreateIndexesAsync();
                await graphRepository.CreateConstraintsAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
