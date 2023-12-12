using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.Targets;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Target.Domain.Aggregates;

namespace Target.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            /* Command */
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<TargetCreatedEvent>();
            BsonClassMap.RegisterClassMap<TargetUpdatedEvent>();
            BsonClassMap.RegisterClassMap<TargetDeletedEvent>();

            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<TargetAggregate>, EventStore<TargetAggregate>>();



            /* Kafka Producer */
            var kafkaProducerSettings = new KafkaProducerSettings
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
                Topic = configuration.GetValue<string>("KafkaProducerSettings:Topic") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic))
            };
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);

            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<TargetAggregate>, EventSourcingHandler<TargetAggregate>>();





            return services;
        }
    }
}