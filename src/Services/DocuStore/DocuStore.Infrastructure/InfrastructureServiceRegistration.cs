

using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.EventStore.Handlers;
using Daikon.Events.DocuStore;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.Shared.APIClients.MLogix;
using DocuStore.Application.Contracts.Persistence;
using DocuStore.Domain.Aggregates;
using DocuStore.Infrastructure.Consumers;
using DocuStore.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace DocuStore.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {

            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            
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

            /* Query */
            services.AddScoped<IParsedDocRepository, ParsedDocRepository>();

            /* Consumers */
            services.AddScoped<IEventConsumer, ParsedDocEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();


            /* MolDb API */
            services.AddScoped<IMLogixAPI, MLogixAPI>();

            return services;
        }
    }
}




