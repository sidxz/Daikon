using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.EventStore.Handlers;
using Daikon.Events.Project;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using Project.Infrastructure.Query.Consumers;
using Project.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Confluent.Kafka;
using Daikon.Shared.APIClients.MLogix;

namespace Project.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            /* MongoDB Conventions */
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            // Register BSON class maps
            RegisterBsonClassMaps();

            /* Event Database */
            var eventDatabaseSettings = GetEventDatabaseSettings(configuration);
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<ISnapshotRepository, SnapshotRepository>();
            services.AddScoped<IEventStore<ProjectAggregate>, EventStore<ProjectAggregate>>();

            /* Kafka Producer */
            var kafkaProducerSettings = GetKafkaProducerSettings(configuration);
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);
            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<ProjectAggregate>, EventSourcingHandler<ProjectAggregate>>();

            /* Query */
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectCompoundEvolutionRepository, ProjectCompoundEvolutionRepository>();

            /* Consumers */
            services.AddScoped<IEventConsumer, ProjectEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API */
            services.AddScoped<IMLogixAPI, MLogixAPI>();

            return services;
        }

        private static void RegisterBsonClassMaps()
        {
            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<ProjectCreatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectDeletedEvent>();
            BsonClassMap.RegisterClassMap<ProjectRenamedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionAddedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionDeletedEvent>();
            BsonClassMap.RegisterClassMap<ProjectAssociationUpdatedEvent>();
        }

        private static EventDatabaseSettings GetEventDatabaseSettings(IConfiguration configuration)
        {
            return new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString")
                                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString), "Event Database connection string is required."),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName")
                                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName), "Event Database name is required."),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName")
                                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName), "Event Database collection name is required.")
            };
        }

        private static KafkaProducerSettings GetKafkaProducerSettings(IConfiguration configuration)
        {
            var kafkaProducerSettings = new KafkaProducerSettings
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers")
                                     ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers), "Kafka BootstrapServers is required."),
                Topic = configuration.GetValue<string>("KafkaProducerSettings:Topic")
                                     ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic), "Kafka Topic is required."),
                SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol") ?? ""),
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = configuration.GetValue<string>("KafkaProducerSettings:ConnectionString")
                                     ?? throw new ArgumentNullException(nameof(KafkaProducerSettings), "Kafka Connection String is required.")
            };

            // Update security protocol if present
            var kafkaProducerSecurityProtocol = configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol");
            if (!string.IsNullOrEmpty(kafkaProducerSecurityProtocol))
            {
                kafkaProducerSettings.SecurityProtocol = Enum.Parse<SecurityProtocol>(kafkaProducerSecurityProtocol);
            }

            return kafkaProducerSettings;
        }
    }
}
