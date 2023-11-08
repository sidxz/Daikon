using CQRS.Core.Domain;
using CQRS.Core.Infrastructure;
using Gene.Infrastructure.Command.Repositories;
using Gene.Infrastructure.Command.Stores;
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

            return services;
        }

    }
}