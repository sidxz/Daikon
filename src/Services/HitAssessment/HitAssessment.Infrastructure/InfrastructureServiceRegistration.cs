
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
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using HitAssessment.Application.Contracts.Infrastructure;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Aggregates;
using HitAssessment.Domain.EntityRevisions;
using HitAssessment.Infrastructure.MLogixAPI;
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

              /* MongoDb */
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();

            BsonClassMap.RegisterClassMap<HaCreatedEvent>();
            BsonClassMap.RegisterClassMap<HaUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HaDeletedEvent>();
            BsonClassMap.RegisterClassMap<HaRenamedEvent>();

            BsonClassMap.RegisterClassMap<HaCompoundEvolutionAddedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionDeletedEvent>();


            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<HaAggregate>, EventStore<HaAggregate>>();


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
            services.AddScoped<IEventSourcingHandler<HaAggregate>, EventSourcingHandler<HaAggregate>>();



            /* Version Store */
            var haVersionStoreSettings = new VersionDatabaseSettings<HitAssessmentRevision>
            {
                ConnectionString = configuration.GetValue<string>("HAMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitAssessmentRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("HAMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitAssessmentRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("HAMongoDbSettings:HitAssessmentRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HitAssessmentRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<HitAssessmentRevision>>(haVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<HitAssessmentRevision>, VersionStoreRepository<HitAssessmentRevision>>();
            services.AddScoped<IVersionHub<HitAssessmentRevision>, VersionHub<HitAssessmentRevision>>();

            var haCompoundEvolutionVersionStoreSettings = new VersionDatabaseSettings<HaCompoundEvolutionRevision>
            {
                ConnectionString = configuration.GetValue<string>("HAMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HaCompoundEvolutionRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("HAMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HaCompoundEvolutionRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("HAMongoDbSettings:HaCompoundEvolutionRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<HaCompoundEvolutionRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<HaCompoundEvolutionRevision>>(haCompoundEvolutionVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<HaCompoundEvolutionRevision>, VersionStoreRepository<HaCompoundEvolutionRevision>>();
            services.AddScoped<IVersionHub<HaCompoundEvolutionRevision>, VersionHub<HaCompoundEvolutionRevision>>();


            /* Query */
            services.AddScoped<IHitAssessmentRepository, HitAssessmentRepository>();
            services.AddScoped<IHaCompoundEvolutionRepository, HaCompoundEvolutionRepository>();


            /* Consumers */
            services.AddScoped<IEventConsumer, HitAssessmentEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API */
            
            services.AddScoped<IMLogixAPIService, MLogixAPIService>();

            return services;
        }
    }
}