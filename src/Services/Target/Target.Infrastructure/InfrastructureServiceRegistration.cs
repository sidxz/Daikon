
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
            /* Command */
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<TargetCreatedEvent>();
            BsonClassMap.RegisterClassMap<TargetUpdatedEvent>();
            BsonClassMap.RegisterClassMap<TargetDeletedEvent>();
            BsonClassMap.RegisterClassMap<TargetAssociatedGenesUpdatedEvent>();

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



            /* Kafka Producer */
            var kafkaProducerSettings = new KafkaProducerSettings
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
                Topic = configuration.GetValue<string>("KafkaProducerSettings:Topic") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic))
            };
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);

            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<TargetAggregate>, EventSourcingHandler<TargetAggregate>>();



            /* Query */
            services.AddScoped<ITargetRepository, TargetRepository>();

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

             /* Consumers */
            services.AddScoped<IEventConsumer, TargetEventConsumer>(); // Depends on IKafkaConsumerSettings; Takes care of both gene and strain events
            services.AddHostedService<ConsumerHostedService>();

            return services;
        }
    }
}