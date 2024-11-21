

using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.DocuStore;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Domain.Aggregates;
using DocuStore.Domain.EntityRevisions;
using DocuStore.Infrastructure.Consumers;
using DocuStore.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace DocuStore.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<ParsedDocAddedEvent>();
            BsonClassMap.RegisterClassMap<ParsedDocUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ParsedDocDeletedEvent>();

            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<ParsedDocAggregate>, EventStore<ParsedDocAggregate>>();

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
            services.AddScoped<IEventSourcingHandler<ParsedDocAggregate>, EventSourcingHandler<ParsedDocAggregate>>();


            /* Version Store */
            var parsedDocVersionStore = new VersionDatabaseSettings<ParsedDocRevision>
            {
                ConnectionString = configuration.GetValue<string>("DocuStoreMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ParsedDocRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("DocuStoreMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ParsedDocRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("DocuStoreMongoDbSettings:ParsedDocRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<ParsedDocRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<ParsedDocRevision>>(parsedDocVersionStore);
            services.AddScoped<IVersionStoreRepository<ParsedDocRevision>, VersionStoreRepository<ParsedDocRevision>>();
            services.AddScoped<IVersionHub<ParsedDocRevision>, VersionHub<ParsedDocRevision>>();


            /* Query */
            services.AddScoped<IParsedDocRepository, ParsedDocRepository>();

            /* Consumers */
            services.AddScoped<IEventConsumer, ParsedDocEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            return services;
        }
    }
}




