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

             /* MongoDb */
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);
            /* Command */

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



            /* Event Database */

            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            
            services.AddScoped<IEventStore<GeneAggregate>, EventStore<GeneAggregate>>();
            services.AddScoped<IEventStore<StrainAggregate>, EventStore<StrainAggregate>>();

            /* Kafka Producer */
            var kafkaProducerSettings = new KafkaProducerSettings
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers") 
                                            ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
                Topic = configuration.GetValue<string>("KafkaProducerSettings:Topic") 
                                            ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic)),

                SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol")?? ""),
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
            services.AddScoped<IEventSourcingHandler<GeneAggregate>, EventSourcingHandler<GeneAggregate>>();
            services.AddScoped<IEventSourcingHandler<StrainAggregate>, EventSourcingHandler<StrainAggregate>>();

            /* Query */

            /* Repositories */
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


            /* Version Store */


            var geneVersionStoreSettings = new VersionDatabaseSettings<GeneRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<GeneRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<GeneRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<GeneRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<GeneRevision>>(geneVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<GeneRevision>, VersionStoreRepository<GeneRevision>>();
            services.AddScoped<IVersionHub<GeneRevision>, VersionHub<GeneRevision>>();

            var essentialityVersionStoreSettings = new VersionDatabaseSettings<EssentialityRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<EssentialityRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<EssentialityRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:EssentialityRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "Essentiality"
            };

            services.AddSingleton<IVersionDatabaseSettings<EssentialityRevision>>(essentialityVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<EssentialityRevision>, VersionStoreRepository<EssentialityRevision>>();
            services.AddScoped<IVersionHub<EssentialityRevision>, VersionHub<EssentialityRevision>>();

            var proteinProductionVersionStoreSettings = new VersionDatabaseSettings<ProteinProductionRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProteinProductionRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProteinProductionRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:ProteinProductionRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "ProteinProduction"
            };
            services.AddSingleton<IVersionDatabaseSettings<ProteinProductionRevision>>(proteinProductionVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ProteinProductionRevision>, VersionStoreRepository<ProteinProductionRevision>>();
            services.AddScoped<IVersionHub<ProteinProductionRevision>, VersionHub<ProteinProductionRevision>>();

            var proteinActivityAssayVersionStoreSettings = new VersionDatabaseSettings<ProteinActivityAssayRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProteinActivityAssayRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProteinActivityAssayRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:ProteinActivityAssayRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "ProteinActivityAssay"
            };
            services.AddSingleton<IVersionDatabaseSettings<ProteinActivityAssayRevision>>(proteinActivityAssayVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ProteinActivityAssayRevision>, VersionStoreRepository<ProteinActivityAssayRevision>>();
            services.AddScoped<IVersionHub<ProteinActivityAssayRevision>, VersionHub<ProteinActivityAssayRevision>>();

            var hypomorphVersionStoreSettings = new VersionDatabaseSettings<HypomorphRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HypomorphRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HypomorphRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:HypomorphRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "Hypomorph"
            };
            services.AddSingleton<IVersionDatabaseSettings<HypomorphRevision>>(hypomorphVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<HypomorphRevision>, VersionStoreRepository<HypomorphRevision>>();
            services.AddScoped<IVersionHub<HypomorphRevision>, VersionHub<HypomorphRevision>>();

            var crispriStrainVersionStoreSettings = new VersionDatabaseSettings<CrispriStrainRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CrispriStrainRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CrispriStrainRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:CrispriStrainRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "CrispriStrain"
            };
            services.AddSingleton<IVersionDatabaseSettings<CrispriStrainRevision>>(crispriStrainVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<CrispriStrainRevision>, VersionStoreRepository<CrispriStrainRevision>>();
            services.AddScoped<IVersionHub<CrispriStrainRevision>, VersionHub<CrispriStrainRevision>>();

            var resistanceMutationVersionStoreSettings = new VersionDatabaseSettings<ResistanceMutationRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ResistanceMutationRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ResistanceMutationRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:ResistanceMutationRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "ResistanceMutation"
            };
            services.AddSingleton<IVersionDatabaseSettings<ResistanceMutationRevision>>(resistanceMutationVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ResistanceMutationRevision>, VersionStoreRepository<ResistanceMutationRevision>>();
            services.AddScoped<IVersionHub<ResistanceMutationRevision>, VersionHub<ResistanceMutationRevision>>();

            var vulnerabilityVersionStoreSettings = new VersionDatabaseSettings<VulnerabilityRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<VulnerabilityRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<VulnerabilityRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:VulnerabilityRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "Vulnerability"
            };
            services.AddSingleton<IVersionDatabaseSettings<VulnerabilityRevision>>(vulnerabilityVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<VulnerabilityRevision>, VersionStoreRepository<VulnerabilityRevision>>();
            services.AddScoped<IVersionHub<VulnerabilityRevision>, VersionHub<VulnerabilityRevision>>();

            var unpublishedStructuralInformationVersionStoreSettings = new VersionDatabaseSettings<UnpublishedStructuralInformationRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<UnpublishedStructuralInformationRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<UnpublishedStructuralInformationRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:UnpublishedStructuralInformationRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "UnpublishedStructuralInformation"
            };
            services.AddSingleton<IVersionDatabaseSettings<UnpublishedStructuralInformationRevision>>(unpublishedStructuralInformationVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<UnpublishedStructuralInformationRevision>, VersionStoreRepository<UnpublishedStructuralInformationRevision>>();
            services.AddScoped<IVersionHub<UnpublishedStructuralInformationRevision>, VersionHub<UnpublishedStructuralInformationRevision>>();


            var expansionPropVersionStoreSettings = new VersionDatabaseSettings<GeneExpansionPropRevision>
            {
                ConnectionString = configuration.GetValue<string>("GeneMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<GeneExpansionPropRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("GeneMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<GeneExpansionPropRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("GeneMongoDbSettings:ExpansionPropRevisionCollectionName")
                ?? configuration.GetValue<string>("GeneMongoDbSettings:GeneRevisionCollectionName") + "ExpansionPropRevision"
            };
            services.AddSingleton<IVersionDatabaseSettings<GeneExpansionPropRevision>>(expansionPropVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<GeneExpansionPropRevision>, VersionStoreRepository<GeneExpansionPropRevision>>();
            services.AddScoped<IVersionHub<GeneExpansionPropRevision>, VersionHub<GeneExpansionPropRevision>>();


            /* Consumers */
            services.AddScoped<IEventConsumer, GeneEventConsumer>(); // Depends on IKafkaConsumerSettings; Takes care of both gene and strain events
            services.AddHostedService<ConsumerHostedService>();


            /* Batch */
            services.AddScoped<IBatchRepositoryOperations, BatchRepositoryOperations>(); // Depends on IMongoCollection<Domain.Entities.Gene> and IMongoCollection<Domain.Entities.Essentiality>

            return services;
        }

    }
}