using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.HitAssessment;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.Shared.APIClients.MLogix;
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Aggregates;
using HitAssessment.Domain.EntityRevisions;
using HitAssessment.Infrastructure.Query.Consumers;
using HitAssessment.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace HitAssessment.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            /* MongoDB Conventions */
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            // Register BSON class maps for HitAssessment events
            RegisterBsonClassMaps();

            /* Event Database */
            var eventDatabaseSettings = GetEventDatabaseSettings(configuration);
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<IEventStore<HaAggregate>, EventStore<HaAggregate>>();

            /* Kafka Producer */
            var kafkaProducerSettings = GetKafkaProducerSettings(configuration);
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);
            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<HaAggregate>, EventSourcingHandler<HaAggregate>>();

            /* Version Store */
            ConfigureVersionStore<HitAssessmentRevision>(services, configuration, "HAMongoDbSettings:HitAssessmentRevisionCollectionName");
            ConfigureVersionStore<HaCompoundEvolutionRevision>(services, configuration, "HAMongoDbSettings:HaCompoundEvolutionRevisionCollectionName");

            /* Query Repositories */
            services.AddScoped<IHitAssessmentRepository, HitAssessmentRepository>();
            services.AddScoped<IHaCompoundEvolutionRepository, HaCompoundEvolutionRepository>();

            /* Event Consumers */
            services.AddScoped<IEventConsumer, HitAssessmentEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API Client */
            services.AddScoped<IMLogixAPI, MLogixAPI>();

            return services;
        }

        private static void RegisterBsonClassMaps()
        {
            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<HaCreatedEvent>();
            BsonClassMap.RegisterClassMap<HaUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HaDeletedEvent>();
            BsonClassMap.RegisterClassMap<HaRenamedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionAddedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionDeletedEvent>();
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

            var kafkaProducerSecurityProtocol = configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol");
            if (!string.IsNullOrEmpty(kafkaProducerSecurityProtocol))
            {
                kafkaProducerSettings.SecurityProtocol = Enum.Parse<SecurityProtocol>(kafkaProducerSecurityProtocol);
            }

            return kafkaProducerSettings;
        }

        private static void ConfigureVersionStore<T>(IServiceCollection services, IConfiguration configuration, string collectionNameKey)
            where T : CQRS.Core.Domain.Historical.BaseVersionEntity
        {
            var versionDatabaseSettings = new VersionDatabaseSettings<T>
            {
                ConnectionString = configuration.GetValue<string>("HAMongoDbSettings:ConnectionString")
                                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<T>.ConnectionString), "Version Store connection string is required."),
                DatabaseName = configuration.GetValue<string>("HAMongoDbSettings:DatabaseName")
                                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<T>.DatabaseName), "Version Store database name is required."),
                CollectionName = configuration.GetValue<string>(collectionNameKey)
                                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<T>.CollectionName), $"Collection name for {typeof(T).Name} is required.")
            };

            services.AddSingleton<IVersionDatabaseSettings<T>>(versionDatabaseSettings);
            services.AddScoped<IVersionStoreRepository<T>, VersionStoreRepository<T>>();
            services.AddScoped<IVersionHub<T>, VersionHub<T>>();
        }
    }
}
