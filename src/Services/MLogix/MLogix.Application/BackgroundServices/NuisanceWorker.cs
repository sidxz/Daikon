
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MLogix.Application.BackgroundServices
{
    public sealed class NuisanceWorker(
    INuisanceJobQueue queue,
    IServiceProvider serviceProvider,
    ILogger<NuisanceWorker> logger
) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("NuisanceWorker started at {Timestamp}.", DateTimeOffset.UtcNow);

            await foreach (var job in queue.DequeueAllAsync(stoppingToken))
            {
                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                using var jobCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                var jobToken = jobCts.Token;

                // log start of job with timestamp, and record total time taken
                var jobStartTime = DateTimeOffset.UtcNow;
                logger.LogInformation("[START] Processing nuisance job. CorrelationId={CorrelationId} at {Timestamp}", job.CorrelationId, jobStartTime);


                try
                {
                    // If you want retries/backoff, you can wrap this with Polly here.
                    await mediator.Send(job.Command, jobToken);

                    var jobEndTime = DateTimeOffset.UtcNow;
                    var duration = (jobEndTime - jobStartTime).TotalSeconds;
                    logger.LogInformation("[COMPLETE] Nuisance job completed. CorrelationId={CorrelationId}, Duration={Duration}s", job.CorrelationId, duration);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("[CANCEL] NuisanceWorker stopping, job canceled. CorrelationId={CorrelationId}", job.CorrelationId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ERROR] Nuisance job failed. CorrelationId={CorrelationId}", job.CorrelationId);
                    // Optional: enqueue to a dead-letter store, or add retry policy.
                }


            }

            logger.LogInformation("NuisanceWorker  stopped at {Timestamp}.", DateTimeOffset.UtcNow);
        }
    }
}