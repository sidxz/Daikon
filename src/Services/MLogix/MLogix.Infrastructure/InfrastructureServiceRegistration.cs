using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.MLogix;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Aggregates;
using MLogix.Domain.EntityRevisions;
using MLogix.Infrastructure.Query.Consumers;
using MLogix.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Confluent.Kafka;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Infrastructure.DaikonChemVault;
using MongoDB.Bson.Serialization.Conventions;

namespace MLogix.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);
            
            RegisterBsonClassMaps();

            ConfigureEventDatabase(services, configuration);
            ConfigureKafkaProducer(services, configuration);
            ConfigureVersionStore(services, configuration);

            /* Query */
            services.AddScoped<IMoleculeRepository, MoleculeRepository>();

            /* Consumers */
            services.AddScoped<IEventConsumer, MLogixEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API */
            services.AddScoped<IMoleculeAPI, MoleculeAPI>();

            return services;
        }

        private static void RegisterBsonClassMaps()
        {
            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<MoleculeCreatedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeUpdatedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeDisclosedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeDeletedEvent>();
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
            services.AddScoped<IEventStore<MoleculeAggregate>, EventStore<MoleculeAggregate>>();
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

            var kafkaProducerSecurityProtocol = configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol");
            if (!string.IsNullOrEmpty(kafkaProducerSecurityProtocol))
            {
                kafkaProducerSettings.SecurityProtocol = Enum.Parse<SecurityProtocol>(kafkaProducerSecurityProtocol);
            }

            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);
            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<MoleculeAggregate>, EventSourcingHandler<MoleculeAggregate>>();
        }

        private static void ConfigureVersionStore(IServiceCollection services, IConfiguration configuration)
        {
            var moleculeVersionStoreSettings = new VersionDatabaseSettings<MoleculeRevision>
            {
                ConnectionString = configuration.GetValue<string>("MLxMongoDbSettings:ConnectionString")
                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<MoleculeRevision>.ConnectionString), "Version Store connection string is required."),
                DatabaseName = configuration.GetValue<string>("MLxMongoDbSettings:DatabaseName")
                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<MoleculeRevision>.DatabaseName), "Version Store database name is required."),
                CollectionName = configuration.GetValue<string>("MLxMongoDbSettings:MoleculeRevisionCollectionName")
                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<MoleculeRevision>.CollectionName), "Version Store collection name is required.")
            };

            services.AddSingleton<IVersionDatabaseSettings<MoleculeRevision>>(moleculeVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<MoleculeRevision>, VersionStoreRepository<MoleculeRevision>>();
            services.AddScoped<IVersionHub<MoleculeRevision>, VersionHub<MoleculeRevision>>();
        }
    }
}
