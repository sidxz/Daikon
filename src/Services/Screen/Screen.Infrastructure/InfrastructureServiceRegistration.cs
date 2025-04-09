using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.Shared.APIClients.MLogix;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Infrastructure.Query.Consumers;
using Screen.Infrastructure.Query.Repositories;
using System.Text.Json;
using Screen.Infrastructure.Query.Converters;
using System.Text.Json.Serialization;

namespace Screen.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();            

            ConfigureMongoDbConventions();
            RegisterBsonClassMaps();

            ConfigureEventDatabase(services, configuration);
            ConfigureKafkaProducer(services, configuration);

            ConfigureRepositories(services);
            ConfigureConsumers(services);

            services.AddScoped<IMLogixAPI, MLogixAPI>();

            return services;
        }

        private static void ConfigureMongoDbConventions()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);
        }

        private static void RegisterBsonClassMaps()
        {
            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<ScreenCreatedEvent>();
            BsonClassMap.RegisterClassMap<ScreenUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ScreenDeletedEvent>();
            BsonClassMap.RegisterClassMap<ScreenRenamedEvent>();
            BsonClassMap.RegisterClassMap<ScreenAssociatedTargetsUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ScreenRunAddedEvent>();
            BsonClassMap.RegisterClassMap<ScreenRunUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ScreenRunDeletedEvent>();
            BsonClassMap.RegisterClassMap<HitCollectionCreatedEvent>();
            BsonClassMap.RegisterClassMap<HitCollectionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HitCollectionDeletedEvent>();
            BsonClassMap.RegisterClassMap<HitCollectionRenamedEvent>();
            BsonClassMap.RegisterClassMap<HitCollectionAssociatedScreenUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HitAddedEvent>();
            BsonClassMap.RegisterClassMap<HitUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HitMoleculeUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HitDeletedEvent>();
        }

        private static void ConfigureEventDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString")
                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString), "Event Database connection string is required."),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName")
                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName), "Event Database name is required."),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName")
                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName), "Event Database collection name is required.")
            };


            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<ISnapshotRepository, SnapshotRepository>();

            services.AddScoped<IEventStore<ScreenAggregate>, EventStore<ScreenAggregate>>();
            services.AddScoped<IEventStore<HitCollectionAggregate>, EventStore<HitCollectionAggregate>>();
        }

        private static void ConfigureKafkaProducer(IServiceCollection services, IConfiguration configuration)
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

            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);
            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<ScreenAggregate>, EventSourcingHandler<ScreenAggregate>>();
            services.AddScoped<IEventSourcingHandler<HitCollectionAggregate>, EventSourcingHandler<HitCollectionAggregate>>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IScreenRepository, ScreenRepository>();
            services.AddScoped<IScreenRunRepository, ScreenRunRepository>();
            services.AddScoped<IHitCollectionRepository, HitCollectionRepository>();
            services.AddScoped<IHitRepository, HitRepository>();
        }

        private static void ConfigureConsumers(IServiceCollection services)
        {
            services.AddScoped<IEventConsumer, ScreenEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();
        }
    }
}
