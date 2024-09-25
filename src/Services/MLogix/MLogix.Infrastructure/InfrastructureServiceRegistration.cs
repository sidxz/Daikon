
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
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Aggregates;
using MLogix.Domain.EntityRevisions;
using MLogix.Infrastructure.MolDbAPI;
using MLogix.Infrastructure.Query.Consumers;
using MLogix.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Confluent.Kafka;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Infrastructure.DaikonChemVault;

namespace MLogix.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<MoleculeCreatedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeUpdatedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeDeletedEvent>();


            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<MoleculeAggregate>, EventStore<MoleculeAggregate>>();


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
            services.AddScoped<IEventSourcingHandler<MoleculeAggregate>, EventSourcingHandler<MoleculeAggregate>>();



            /* Version Store */
            var moleculeVersionStoreSettings = new VersionDatabaseSettings<MoleculeRevision>
            {
                ConnectionString = configuration.GetValue<string>("MLxMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<MoleculeRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("MLxMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<MoleculeRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("MLxMongoDbSettings:MoleculeRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<MoleculeRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<MoleculeRevision>>(moleculeVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<MoleculeRevision>, VersionStoreRepository<MoleculeRevision>>();
            services.AddScoped<IVersionHub<MoleculeRevision>, VersionHub<MoleculeRevision>>();

          
            /* Query */
            services.AddScoped<IMoleculeRepository, MoleculeRepository>();


            /* Consumers */
            services.AddScoped<IEventConsumer, MLogixEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API */
            services.AddScoped<IMolDbAPIService, MolDbAPIService>();
            services.AddScoped<IMoleculeAPI, MoleculeAPI>();

            return services;
        }
    }
}