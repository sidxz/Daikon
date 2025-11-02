
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MLogix.Application.Features.Batch.RefreshAllNuisancePredictions;

namespace MLogix.Application.BackgroundServices
{
    public class MLogixBackgroundServices(IServiceProvider serviceProvider, ILogger<MLogixBackgroundServices> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<MLogixBackgroundServices> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This method can be used for recurring tasks, but we'll leave it empty
            // as we will queue specific jobs from the controller.
        }

        public async Task QueueRefreshNuisance(RefreshAllNuisancePredictionsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("RefreshNuisance job has been queued at {Timestamp}.", DateTimeOffset.UtcNow);
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            using var jobCts = new CancellationTokenSource();
            var jobToken = jobCts.Token;

            try
            {
                await mediator.Send(command, jobToken);
                _logger.LogInformation("RefreshNuisance job completed successfully at {Timestamp}.", DateTimeOffset.UtcNow);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("RefreshNuisance job was canceled (token triggered).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the RefreshNuisance job.");
            }
        }

    }
}