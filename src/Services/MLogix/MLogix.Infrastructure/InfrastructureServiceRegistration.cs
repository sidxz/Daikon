using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.EventStore.Handlers;
using Daikon.Events.MLogix;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using MLogix.Application.Contracts.Persistence;
using MLogix.Domain.Aggregates;
using MLogix.Infrastructure.Query.Consumers;
using MLogix.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Confluent.Kafka;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Infrastructure.DaikonChemVault;
using MongoDB.Bson.Serialization.Conventions;
using MLogix.Application.Contracts.Infrastructure.CageFusion;
using MLogix.Infrastructure.CageFusion;
using Daikon.Shared.Constants.InternalSettings;

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
            /* Query */
            services.AddScoped<IMoleculeRepository, MoleculeRepository>();
            services.AddScoped<IMoleculePredictionRepository, MoleculePredictionsRepository>();

            /* Consumers */
            services.AddScoped<IEventConsumer, MLogixEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            /* MolDb API */
            services.AddScoped<IMoleculeAPI, MoleculeAPI>();
            /* Nuisance API */

            services.AddHttpClient<INuisanceAPI, NuisanceAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["CageFusion:NuisanceAPIURL"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = Timeouts.HttpClientTimeout;
            });


            //services.AddScoped<INuisanceAPI, NuisanceAPI>();

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
            BsonClassMap.RegisterClassMap<MoleculeNuisancePredictedEvent>();
        }

        private static void ConfigureEventDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString")
                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString), "Event Database connection string is required."),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName")
                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName), "Event Database name is required."),
            };

            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<ISnapshotRepository, SnapshotRepository>();
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

    }
}
