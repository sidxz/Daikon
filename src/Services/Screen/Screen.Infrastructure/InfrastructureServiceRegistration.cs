
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.Screens;
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
using Screen.Application.Contracts.Infrastructure;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.EntityRevisions;
using Screen.Infrastructure.MLogixAPI;
using Screen.Infrastructure.MolDbAPI;
using Screen.Infrastructure.Query.Consumers;
using Screen.Infrastructure.Query.Repositories;

namespace Screen.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {

            /* MongoDb */
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            
            /* Command */
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
            BsonClassMap.RegisterClassMap<HitDeletedEvent>();


            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<ScreenAggregate>, EventStore<ScreenAggregate>>();
            services.AddScoped<IEventStore<HitCollectionAggregate>, EventStore<HitCollectionAggregate>>();




            /* Kafka Producer */
            var kafkaProducerSettings = new KafkaProducerSettings
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
                Topic = configuration.GetValue<string>("KafkaProducerSettings:Topic") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic))
            };
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);

            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<ScreenAggregate>, EventSourcingHandler<ScreenAggregate>>();
            services.AddScoped<IEventSourcingHandler<HitCollectionAggregate>, EventSourcingHandler<HitCollectionAggregate>>();



            /* Query */
            services.AddScoped<IScreenRepository, ScreenRepository>();
            services.AddScoped<IScreenRunRepository, ScreenRunRepository>();
            services.AddScoped<IHitCollectionRepository, HitCollectionRepository>();
            services.AddScoped<IHitRepository, HitRepository>();

            /* Version Store */
            var screenVersionStoreSettings = new VersionDatabaseSettings<ScreenRevision>
            {
                ConnectionString = configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ScreenRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ScreenRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("ScreenMongoDbSettings:ScreenRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ScreenRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<ScreenRevision>>(screenVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ScreenRevision>, VersionStoreRepository<ScreenRevision>>();
            services.AddScoped<IVersionHub<ScreenRevision>, VersionHub<ScreenRevision>>();


            var screenRunVersionStoreSettings = new VersionDatabaseSettings<ScreenRunRevision>
            {
                ConnectionString = configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ScreenRunRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ScreenRunRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("ScreenMongoDbSettings:ScreenRunRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ScreenRunRevision>.CollectionName))
            };

            services.AddSingleton<IVersionDatabaseSettings<ScreenRunRevision>>(screenRunVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ScreenRunRevision>, VersionStoreRepository<ScreenRunRevision>>();
            services.AddScoped<IVersionHub<ScreenRunRevision>, VersionHub<ScreenRunRevision>>();

            var hitCollectionVersionStoreSettings = new VersionDatabaseSettings<HitCollectionRevision>
            {
                ConnectionString = configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitCollectionRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitCollectionRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("ScreenMongoDbSettings:HitCollectionRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitCollectionRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<HitCollectionRevision>>(hitCollectionVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<HitCollectionRevision>, VersionStoreRepository<HitCollectionRevision>>();
            services.AddScoped<IVersionHub<HitCollectionRevision>, VersionHub<HitCollectionRevision>>();

            var hitVersionStoreSettings = new VersionDatabaseSettings<HitRevision>
            {
                ConnectionString = configuration.GetValue<string>("ScreenMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("ScreenMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("ScreenMongoDbSettings:HitRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<HitRevision>>(hitVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<HitRevision>, VersionStoreRepository<HitRevision>>();
            services.AddScoped<IVersionHub<HitRevision>, VersionHub<HitRevision>>();


            /* Consumers */
            services.AddScoped<IEventConsumer, ScreenEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API */
            services.AddScoped<IMolDbAPIService, MolDbAPIService>();
            services.AddScoped<IMLogixAPIService, MLogixAPIService>();

            return services;
        }
    }
}