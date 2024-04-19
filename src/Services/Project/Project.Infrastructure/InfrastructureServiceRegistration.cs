
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.Project;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using Project.Application.Contracts.Infrastructure;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using Project.Domain.EntityRevisions;
using Project.Infrastructure.Query.Consumers;
using Project.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Project.Infrastructure.MLogixAPI;
using MongoDB.Bson.Serialization.Conventions;
using Confluent.Kafka;

namespace Project.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {

            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<ProjectCreatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectDeletedEvent>();

            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionAddedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionDeletedEvent>();

            BsonClassMap.RegisterClassMap<ProjectAssociationUpdatedEvent>();


            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<ProjectAggregate>, EventStore<ProjectAggregate>>();


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
            services.AddScoped<IEventSourcingHandler<ProjectAggregate>, EventSourcingHandler<ProjectAggregate>>();



            /* Version Store */
            var projectVersionStoreSettings = new VersionDatabaseSettings<ProjectRevision>
            {
                ConnectionString = configuration.GetValue<string>("ProjectMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProjectRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("ProjectMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProjectRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("ProjectMongoDbSettings:ProjectRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProjectRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<ProjectRevision>>(projectVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ProjectRevision>, VersionStoreRepository<ProjectRevision>>();
            services.AddScoped<IVersionHub<ProjectRevision>, VersionHub<ProjectRevision>>();

            var projectCompoundEvolutionVersionStoreSettings = new VersionDatabaseSettings<ProjectCompoundEvolutionRevision>
            {
                ConnectionString = configuration.GetValue<string>("ProjectMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProjectCompoundEvolutionRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("ProjectMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProjectCompoundEvolutionRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("ProjectMongoDbSettings:ProjectCompoundEvolutionRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ProjectCompoundEvolutionRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<ProjectCompoundEvolutionRevision>>(projectCompoundEvolutionVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<ProjectCompoundEvolutionRevision>, VersionStoreRepository<ProjectCompoundEvolutionRevision>>();
            services.AddScoped<IVersionHub<ProjectCompoundEvolutionRevision>, VersionHub<ProjectCompoundEvolutionRevision>>();


            /* Query */
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectCompoundEvolutionRepository, ProjectCompoundEvolutionRepository>();


            /* Consumers */
            services.AddScoped<IEventConsumer, ProjectEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API */
            services.AddScoped<IMLogixAPIService, MLogixAPIService>();

            return services;
        }
    }
}