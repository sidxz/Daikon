
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Screen.Application.Features.Batch.HitHandleUnattachedMolecules;

namespace Screen.Application.BackgroundServices
{
    public class HitBackgroundService(IServiceProvider serviceProvider, ILogger<HitBackgroundService> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<HitBackgroundService> _logger = logger;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This background service only runs on-demand via queued jobs
            _logger.LogInformation("HitBackgroundService started. Waiting for jobs to be queued.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Queues a background job to handle unattached hits by sending the corresponding command via MediatR.
        /// </summary>
        public async Task QueueHandleUnattachedHitsJobAsync(HitHandleUnattachedMoleculesCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("HitHandleUnattachedMolecules job has been queued.");

            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            try
            {
                await mediator.Send(command, cancellationToken);
                _logger.LogInformation("HitHandleUnattachedMolecules job completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the HitHandleUnattachedMolecules job.");
            }
        }
    }
}
