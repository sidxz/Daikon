using CQRS.Core.Consumers;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using Gene.Infrastructure.Command.Handlers;
using Gene.Infrastructure.Command.Producers;
using Gene.Infrastructure.Command.Repositories;
using Gene.Infrastructure.Command.Stores;
using Gene.Infrastructure.Query.Consumers;
using Gene.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Gene.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<IEventStore, EventStore>();
            services.AddScoped<IEventProducer, EventProducer>();
            services.AddScoped<IEventSourcingHandler<GeneAggregate>, EventSourcingHandler>();

            services.AddScoped<IGeneRepository, GeneRepository>();
            services.AddScoped<IEventConsumer, EventConsumer>();

            services.AddHostedService<ConsumerHostedService>();

            return services;
        }

    }
}