using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.EventStore.Handlers;
using Daikon.Events.Targets;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Target.Application.Contracts.Persistence;
using Target.Domain.Aggregates;
using Target.Infrastructure.Query.Consumers;
using Target.Infrastructure.Query.Repositories;

namespace Target.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureMongoDbConventions();
            RegisterBsonClassMaps();

            ConfigureEventDatabase(services, configuration);
            ConfigureKafkaProducer(services, configuration);
            ConfigureRepositories(services);
            ConfigureConsumers(services);

            return services;
        }

        private static void ConfigureMongoDbConventions()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);
        }

        private static void RegisterBsonClassMaps()
        {
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<TargetCreatedEvent>();
            BsonClassMap.RegisterClassMap<TargetUpdatedEvent>();
            BsonClassMap.RegisterClassMap<TargetDeletedEvent>();
            BsonClassMap.RegisterClassMap<TargetAssociatedGenesUpdatedEvent>();
            BsonClassMap.RegisterClassMap<TargetRenamedEvent>();
            BsonClassMap.RegisterClassMap<TargetPromotionQuestionnaireSubmittedEvent>();
            BsonClassMap.RegisterClassMap<TargetPromotionQuestionnaireUpdatedEvent>();
            BsonClassMap.RegisterClassMap<TargetPromotionQuestionnaireDeletedEvent>();
            BsonClassMap.RegisterClassMap<TargetToxicologyAddedEvent>();
            BsonClassMap.RegisterClassMap<TargetToxicologyUpdatedEvent>();
            BsonClassMap.RegisterClassMap<TargetToxicologyDeletedEvent>();
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
            services.AddScoped<IEventStore<TargetAggregate>, EventStore<TargetAggregate>>();
            services.AddScoped<IEventStore<TPQuestionnaireAggregate>, EventStore<TPQuestionnaireAggregate>>();
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
            services.AddScoped<IEventSourcingHandler<TargetAggregate>, EventSourcingHandler<TargetAggregate>>();
            services.AddScoped<IEventSourcingHandler<TPQuestionnaireAggregate>, EventSourcingHandler<TPQuestionnaireAggregate>>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<ITargetRepository, TargetRepository>();
            services.AddScoped<IPQResponseRepository, PQResponseRepository>();
            services.AddScoped<IToxicologyRepo, ToxicologyRepo>();
        }

        private static void ConfigureConsumers(IServiceCollection services)
        {
            services.AddScoped<IEventConsumer, TargetEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();
        }
    }
}
