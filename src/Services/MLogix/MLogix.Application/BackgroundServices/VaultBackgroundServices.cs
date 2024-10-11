
using Microsoft.Extensions.Logging;
using MLogix.Application.Features.Commands.ReregisterVault;
using Microsoft.Extensions.Hosting;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MLogix.Application.Features.Commands.GenerateParentBatch;

namespace MLogix.Application.BackgroundServices
{
    public class VaultBackgroundServices(IServiceProvider serviceProvider, ILogger<VaultBackgroundServices> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<VaultBackgroundServices> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This method can be used for recurring tasks, but we'll leave it empty
            // as we will queue specific jobs from the controller.
        }

        public async Task QueueReregisterVaultJobAsync(ReRegisterVaultCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReregisterVault job has been queued.");
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            try
            {
                await mediator.Send(command, cancellationToken);
                _logger.LogInformation("ReregisterVault job completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the ReregisterVault job.");
            }
        }

        public async Task QueueGenerateParentBatchJobAsync(GenerateParentBatchCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GenerateParentBatch job has been queued.");
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            try
            {
                await mediator.Send(command, cancellationToken);
                _logger.LogInformation("GenerateParentBatch job completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the GenerateParentBatch job.");
            }
        }


    }
}