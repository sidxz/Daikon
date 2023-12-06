using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Consumers;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Horizon.Infrastructure.Query.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly string _topic;

        public ConsumerHostedService(IServiceProvider serviceProvider, ILogger<ConsumerHostedService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(_serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _topic = configuration.GetValue<string>("KafkaConsumerSettings:Topic") ?? throw new ArgumentNullException(nameof(_topic));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Horizon-gene consumer hosted service for topic: {@topic}", _topic);

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                try {
                    Task.Run(() => eventConsumer.Consume(_topic), cancellationToken);
                }
                catch (EventConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming event");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming event");
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Horizon-gene consumer hosted service");
            return Task.CompletedTask;
        }
    }
}