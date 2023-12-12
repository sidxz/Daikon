
using CQRS.Core.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Target.Infrastructure.Query.Consumers
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
            _logger.LogInformation("Starting target consumer hosted service");

            var scope = _serviceProvider.CreateScope(); // Store scope to dispose of later
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

            // Run the consuming in a separate task
            _ = Task.Run(() =>
            {
                try
                {
                    eventConsumer.Consume(_topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming event");
                }
                finally
                {
                    scope.Dispose(); // Ensure to dispose of the scope when the task is done
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping target consumer hosted service");
            return Task.CompletedTask;
        }
    }
}