using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.Gene;
using Daikon.Events.Strains;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using Gene.Application.Contracts.Infrastructure;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Domain.EntityRevisions;
using Gene.Infrastructure.Batch;
using Gene.Infrastructure.Query.Consumers;
using Gene.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Gene.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            /* MongoDB Conventions */
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            // Register BSON class maps for Gene and Strain events
            RegisterBsonClassMaps();

            /* Event Database */
            var eventDatabaseSettings = GetEventDatabaseSettings(configuration);
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<IEventStore<GeneAggregate>, EventStore<GeneAggregate>>();
            services.AddScoped<IEventStore<StrainAggregate>, EventStore<StrainAggregate>>();

            /* Kafka Producer */
            var kafkaProducerSettings = GetKafkaProducerSettings(configuration);
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);
            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<GeneAggregate>, EventSourcingHandler<GeneAggregate>>();
            services.AddScoped<IEventSourcingHandler<StrainAggregate>, EventSourcingHandler<StrainAggregate>>();

            /* Version Store */
            ConfigureVersionStore<GeneRevision>(services, configuration, "GeneMongoDbSettings:GeneRevisionCollectionName");
            ConfigureVersionStore<EssentialityRevision>(services, configuration, "GeneMongoDbSettings:EssentialityRevisionCollectionName", "Essentiality");
            ConfigureVersionStore<ProteinProductionRevision>(services, configuration, "GeneMongoDbSettings:ProteinProductionRevisionCollectionName", "ProteinProduction");
            ConfigureVersionStore<ProteinActivityAssayRevision>(services, configuration, "GeneMongoDbSettings:ProteinActivityAssayRevisionCollectionName", "ProteinActivityAssay");
            ConfigureVersionStore<HypomorphRevision>(services, configuration, "GeneMongoDbSettings:HypomorphRevisionCollectionName", "Hypomorph");
            ConfigureVersionStore<CrispriStrainRevision>(services, configuration, "GeneMongoDbSettings:CrispriStrainRevisionCollectionName", "CrispriStrain");
            ConfigureVersionStore<ResistanceMutationRevision>(services, configuration, "GeneMongoDbSettings:ResistanceMutationRevisionCollectionName", "ResistanceMutation");
            ConfigureVersionStore<VulnerabilityRevision>(services, configuration, "GeneMongoDbSettings:VulnerabilityRevisionCollectionName", "Vulnerability");
            ConfigureVersionStore<UnpublishedStructuralInformationRevision>(services, configuration, "GeneMongoDbSettings:UnpublishedStructuralInformationRevisionCollectionName", "UnpublishedStructuralInformation");
            ConfigureVersionStore<GeneExpansionPropRevision>(services, configuration, "GeneMongoDbSettings:ExpansionPropRevisionCollectionName", "ExpansionPropRevision");

            /* Repositories */
            ConfigureRepositories(services);

            /* Consumers */
            services.AddScoped<IEventConsumer, GeneEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* Batch Operations */
            services.AddScoped<IBatchRepositoryOperations, BatchRepositoryOperations>();

            return services;
        }

        private static void RegisterBsonClassMaps()
        {
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<GeneCreatedEvent>();
            BsonClassMap.RegisterClassMap<GeneUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneDeletedEvent>();
            BsonClassMap.RegisterClassMap<StrainCreatedEvent>();
            BsonClassMap.RegisterClassMap<StrainUpdatedEvent>();
            BsonClassMap.RegisterClassMap<StrainDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneEssentialityAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneEssentialityUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneEssentialityDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinProductionAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinProductionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinProductionDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinActivityAssayAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinActivityAssayUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinActivityAssayDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneHypomorphAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneHypomorphUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneHypomorphDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneCrispriStrainAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneCrispriStrainUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneCrispriStrainDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneResistanceMutationAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneResistanceMutationUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneResistanceMutationDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneVulnerabilityAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneVulnerabilityUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneVulnerabilityDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneUnpublishedStructuralInformationAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneUnpublishedStructuralInformationUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneUnpublishedStructuralInformationDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneExpansionPropAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneExpansionPropUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneExpansionPropDeletedEvent>();
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

        private static void ConfigureVersionStore<T>(IServiceCollection services, IConfiguration configuration, string collectionNameKey, string defaultSuffix = "")
            where T : CQRS.Core.Domain.Historical.BaseVersionEntity
        {
            var collectionName = configuration.GetValue<string>(collectionNameKey)
                               ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + defaultSuffix;

            var versionDatabaseSettings = new VersionDatabaseSettings<T>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString")
                                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<T>.ConnectionString), "Version Store connection string is required."),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName")
                                    ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<T>.DatabaseName), "Version Store database name is required."),
                CollectionName = collectionName
            };

            services.AddSingleton<IVersionDatabaseSettings<T>>(versionDatabaseSettings);
            services.AddScoped<IVersionStoreRepository<T>, VersionStoreRepository<T>>();
            services.AddScoped<IVersionHub<T>, VersionHub<T>>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IGeneRepository, GeneRepository>();
            services.AddScoped<IStrainRepository, StrainRepository>();
            services.AddScoped<IGeneEssentialityRepository, GeneEssentialityRepository>();
            services.AddScoped<IGeneProteinProductionRepository, GeneProteinProductionRepository>();
            services.AddScoped<IGeneProteinActivityAssayRepository, GeneProteinActivityAssayRepository>();
            services.AddScoped<IGeneHypomorphRepository, GeneHypomorphRepository>();
            services.AddScoped<IGeneCrispriStrainRepository, GeneCrispriStrainRepository>();
            services.AddScoped<IGeneResistanceMutationRepository, GeneResistanceMutationRepository>();
            services.AddScoped<IGeneVulnerabilityRepository, GeneVulnerabilityRepository>();
            services.AddScoped<IGeneUnpublishedStructuralInformationRepository, GeneUnpublishedStructuralInformationRepository>();
            services.AddScoped<IGeneExpansionPropRepo, GeneExpansionPropRepo>();
        }
    }
}
