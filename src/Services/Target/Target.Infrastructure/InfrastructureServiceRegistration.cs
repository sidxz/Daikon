
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.Targets;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Target.Application.Contracts.Persistence;
using Target.Domain.Aggregates;
using Target.Domain.EntityRevisions;
using Target.Infrastructure.Query.Consumers;
using Target.Infrastructure.Query.Repositories;

namespace Target.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            /* Command */
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

            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<TargetAggregate>, EventStore<TargetAggregate>>();
            services.AddScoped<IEventStore<TPQuestionnaireAggregate>, EventStore<TPQuestionnaireAggregate>>();


            /* Kafka Producer */
            var kafkaProducerSettings = new KafkaProducerSettings
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers")
                                            ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
                Topic = configuration.GetValue<string>("KafkaProducerSettings:Topic")
                                            ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic)),

                SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol") ?? ""),
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = configuration.GetValue<string>("KafkaProducerSettings:ConnectionString"),
            };

            var kafkaProducerSecurityProtocol = configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol");
            if (!string.IsNullOrEmpty(kafkaProducerSecurityProtocol))
            {
                kafkaProducerSettings.SecurityProtocol = Enum.Parse<SecurityProtocol>(kafkaProducerSecurityProtocol);
            }
            var kafkaProducerConnectionString = configuration.GetValue<string>("KafkaProducerSettings:ConnectionString");
            if (!string.IsNullOrEmpty(kafkaProducerConnectionString))
            {
                kafkaProducerSettings.SaslMechanism = SaslMechanism.Plain;
                kafkaProducerSettings.SaslUsername = "$ConnectionString";
                kafkaProducerSettings.SaslPassword = kafkaProducerConnectionString;
            }
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);

            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<TargetAggregate>, EventSourcingHandler<TargetAggregate>>();
            services.AddScoped<IEventSourcingHandler<TPQuestionnaireAggregate>, EventSourcingHandler<TPQuestionnaireAggregate>>();


            /* Query */
            services.AddScoped<ITargetRepository, TargetRepository>();
            services.AddScoped<IPQResponseRepository, PQResponseRepository>();
            services.AddScoped<IToxicologyRepo, ToxicologyRepo>();

            /* Version Store */
            var targetVersionStoreSettings = new VersionDatabaseSettings<TargetRevision>
            {
                ConnectionString = configuration.GetValue<string>("TargetMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<TargetRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("TargetMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<TargetRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("TargetMongoDbSettings:TargetRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<TargetRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<TargetRevision>>(targetVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<TargetRevision>, VersionStoreRepository<TargetRevision>>();
            services.AddScoped<IVersionHub<TargetRevision>, VersionHub<TargetRevision>>();

            var toxicologyVersionStoreSettings = new VersionDatabaseSettings<ToxicologyRevision>
            {
                ConnectionString = configuration.GetValue<string>("TargetMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ToxicologyRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("TargetMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ToxicologyRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("TargetMongoDbSettings:TargetToxicologyRevisionCollectionName") ?? "TargetToxicologiesRevisions"
            };
            services.AddSingleton<IVersionDatabaseSettings<ToxicologyRevision>>(toxicologyVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ToxicologyRevision>, VersionStoreRepository<ToxicologyRevision>>();
            services.AddScoped<IVersionHub<ToxicologyRevision>, VersionHub<ToxicologyRevision>>();

            /* Consumers */
            services.AddScoped<IEventConsumer, TargetEventConsumer>(); // Depends on IKafkaConsumerSettings; Takes care of both gene and strain events
            services.AddHostedService<ConsumerHostedService>();

            return services;
        }
    }
}