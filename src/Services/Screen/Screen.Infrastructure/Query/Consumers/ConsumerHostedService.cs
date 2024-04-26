
using CQRS.Core.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/*
 * ConsumerHostedService Class
 * 
 * Purpose:
 * --------
 * The ConsumerHostedService class is designed to integrate Kafka message consumption into the .NET Core hosting lifecycle. 
 * It acts as a hosted service that starts and stops alongside the application, ensuring that Kafka message consumption 
 * is managed as part of the application's lifecycle. This class is specifically tailored for consuming messages from a 
 * designated Kafka topic, as part of a CQRS-based architecture.
 * 
 * Key Functionalities:
 * --------------------
 * 1. Integration with .NET Core Hosting: Implements IHostedService, allowing it to start and stop with the application.
 * 2. Kafka Topic Consumption: Configures and starts a Kafka consumer to consume messages from a specified topic.
 * 3. Dependency Injection: Utilizes IServiceProvider for resolving dependencies, particularly IEventConsumer for message processing.
 * 4. Configuration Management: Reads Kafka topic configuration from IConfiguration, ensuring external configuration of consumer behavior.
 * 5. Asynchronous Execution: Runs the message consumption loop in a separate background task to avoid blocking the main thread.
 * 6. Scoped Service Handling: Creates a service scope for resolving IEventConsumer, ensuring correct scope handling for dependency injection.
 * 7. Logging: Provides essential logging throughout the service's lifecycle for monitoring and troubleshooting.
 * 
 * Usage:
 * ------
 * This class is automatically instantiated and managed by the .NET Core host if registered in the service collection. 
 * It should be configured in the application's startup class, typically in ConfigureServices, to be part of the application's hosted services.
 * 
 */

namespace Screen.Infrastructure.Query.Consumers
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
            _topic = configuration.GetValue<string>("KafkaConsumerSettings:Topics") ?? throw new ArgumentNullException(nameof(_topic));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Starting] Screen consumer hosted service for topic: {@topic}", _topic);

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
            _logger.LogInformation("[Stopping] Screen consumer hosted service");
            return Task.CompletedTask;
        }
    }
}