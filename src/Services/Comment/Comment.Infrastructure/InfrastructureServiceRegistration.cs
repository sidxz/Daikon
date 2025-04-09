using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;
using Daikon.EventStore.Handlers;
using Daikon.Events.Comment;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Aggregates;
using Comment.Infrastructure.Query.Consumers;
using Comment.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Confluent.Kafka;


namespace Comment.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            BsonClassMap.RegisterClassMap<DocMetadata>();
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<CommentCreatedEvent>();
            BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
            BsonClassMap.RegisterClassMap<CommentDeletedEvent>();

            BsonClassMap.RegisterClassMap<CommentReplyAddedEvent>();
            BsonClassMap.RegisterClassMap<CommentReplyUpdatedEvent>();
            BsonClassMap.RegisterClassMap<CommentReplyDeletedEvent>();
    
            /* Event Database */
            var eventDatabaseSettings = new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName)),
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings
            services.AddScoped<ISnapshotRepository, SnapshotRepository>();

            services.AddScoped<IEventStore<CommentAggregate>, EventStore<CommentAggregate>>();

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
            services.AddScoped<IEventSourcingHandler<CommentAggregate>, EventSourcingHandler<CommentAggregate>>();

            /* Query */
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentReplyRepository, CommentReplyRepository>();

            /* Consumers */
            services.AddScoped<IEventConsumer, CommentEventConsumer>();
            services.AddHostedService<ConsumerHostedService>();

            return services;
        }
    }
}



        
