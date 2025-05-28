using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHistory.Application.Features.Queries.GetEventHistory;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.BackgroundServices
{
    public class DefaultEventHistoryPrepBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DefaultEventHistoryPrepBackgroundService> _logger;

        private readonly IMediator _mediator;
        private Timer _timer;

        public DefaultEventHistoryPrepBackgroundService(IServiceProvider serviceProvider,
            ILogger<DefaultEventHistoryPrepBackgroundService> logger
            , IMediator mediator)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async _ => await GenerateDefaultEventHistory(stoppingToken), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
            return Task.CompletedTask;
        }

        private async Task GenerateDefaultEventHistory(CancellationToken cancellationToken)
        {

            try
            {
                _logger.LogInformation("Refreshing Event History cache...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    // Construct and send the query
                    var query = new GetEventHistoryQuery
                    {
                        RefreshCache = true
                    };

                    var eventHistory = await mediator.Send(query, cancellationToken);
                    _logger.LogInformation("Event History cache refreshed successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing the Event History cache.");
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}