using Horizon.Application.Features.Command.Patches.HandleDuplicates;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RemoveDuplicateMoleculeRelationsBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RemoveDuplicateMoleculeRelationsBackgroundService> _logger;

    public RemoveDuplicateMoleculeRelationsBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RemoveDuplicateMoleculeRelationsBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RemoveDuplicateMoleculeRelationsBackgroundService started.");

        // Run immediately on startup
        await RemoveDuplicates(stoppingToken);

        // Then repeat every 30 minutes
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                await RemoveDuplicates(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown, ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in RemoveDuplicateMoleculeRelationsBackgroundService loop.");
            }
        }
    }

    private async Task RemoveDuplicates(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Background Service: Removing duplicate molecule relations...");

            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new HandleDuplicatesMolRelationsCommand();
            await mediator.Send(command, stoppingToken);

            _logger.LogInformation("Background Service: Duplicate molecule relations removed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while removing duplicate molecule relations.");
        }
    }
}
