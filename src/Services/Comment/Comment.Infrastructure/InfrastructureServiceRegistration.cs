using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.Events.Comment;
using Daikon.EventStore.Handlers;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Stores;
using Daikon.VersionStore.Handlers;
using Daikon.VersionStore.Repositories;
using Daikon.VersionStore.Settings;
using Comment.Application.Contracts.Persistence;
using Comment.Domain.Aggregates;
using Comment.Domain.EntityRevisions;
using Comment.Infrastructure.Query.Consumers;
using Comment.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;


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
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName") ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName))
            };
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on IEventDatabaseSettings

            services.AddScoped<IEventStore<CommentAggregate>, EventStore<CommentAggregate>>();

            /* Kafka Producer */
            var kafkaProducerSettings = new KafkaProducerSettings
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
                Topic = configuration.GetValue<string>("KafkaProducerSettings:Topic") ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic))
            };
            services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);

            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<CommentAggregate>, EventSourcingHandler<CommentAggregate>>();


            /* Version Store */
            var commentVersionStoreSettings = new VersionDatabaseSettings<CommentRevision>
            {
                ConnectionString = configuration.GetValue<string>("CommentMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CommentRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("CommentMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CommentRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("CommentMongoDbSettings:CommentRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CommentRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<CommentRevision>>(commentVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<CommentRevision>, VersionStoreRepository<CommentRevision>>(); 
            services.AddScoped<IVersionHub<CommentRevision>, VersionHub<CommentRevision>>();

            var  commentReplyVersionStoreSettings = new VersionDatabaseSettings<CommentReplyRevision>
            {
                ConnectionString = configuration.GetValue<string>("CommentMongoDbSettings:ConnectionString") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CommentReplyRevision>.ConnectionString)),
                DatabaseName = configuration.GetValue<string>("CommentMongoDbSettings:DatabaseName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CommentReplyRevision>.DatabaseName)),
                CollectionName = configuration.GetValue<string>("CommentMongoDbSettings:CommentReplyRevisionCollectionName") ?? throw new ArgumentNullException(nameof(VersionDatabaseSettings<CommentReplyRevision>.CollectionName))
            };
            services.AddSingleton<IVersionDatabaseSettings<CommentReplyRevision>>(commentReplyVersionStoreSettings);
            services.AddScoped<IVersionStoreRepository<CommentReplyRevision>, VersionStoreRepository<CommentReplyRevision>>();
            services.AddScoped<IVersionHub<CommentReplyRevision>, VersionHub<CommentReplyRevision>>();

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



        
