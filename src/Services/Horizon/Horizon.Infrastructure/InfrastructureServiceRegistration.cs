
using CQRS.Core.Consumers;
using Horizon.Application.Contracts.Persistence;
using Horizon.Infrastructure.HostedServices;
using Horizon.Infrastructure.Query.Consumers;
using Horizon.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace Horizon.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            /* Command */

            services.AddScoped<IGraphRepositoryForGene, GraphRepositoryForGene>();
            services.AddScoped<IGraphRepositoryForTarget, GraphRepositoryForTarget>();
            services.AddScoped<IGraphRepositoryForScreen, GraphRepositoryForScreen>();
            services.AddScoped<IGraphRepositoryForHitCollection, GraphRepositoryForHitCollection>();
            services.AddScoped<IGraphRepositoryForMLogix, GraphRepositoryForMLogix>();
            services.AddScoped<IGraphQueryRepository, GraphQueryRepository>();
            services.AddScoped<IHitAssessmentRepo, HitAssessmentRepo>();
            services.AddScoped<IProjectRepo, ProjectRepo>();


            string neo4jUri = configuration.GetValue<string>("HorizonNeo4jSettings:Uri") ?? throw new ArgumentNullException(nameof(neo4jUri));
            string neo4jUser = configuration.GetValue<string>("HorizonNeo4jSettings:User") ?? throw new ArgumentNullException(nameof(neo4jUser));
            string neo4jPassword = configuration.GetValue<string>("HorizonNeo4jSettings:Password") ?? throw new ArgumentNullException(nameof(neo4jPassword));




            services.AddSingleton<IDriver>(sp =>
            {
                return GraphDatabase.Driver(
                    neo4jUri,
                    AuthTokens.Basic(neo4jUser, neo4jPassword),
                    new Action<ConfigBuilder>(config =>
                    {
                        config.WithMaxConnectionPoolSize(120); 
                        config.WithConnectionTimeout(TimeSpan.FromSeconds(120)); 
                        config.WithMaxTransactionRetryTime(TimeSpan.FromSeconds(120)); 
                        config.WithEncryptionLevel(EncryptionLevel.None);
                    })
                );
            });

            services.AddHostedService<Neo4jDatabaseSetupHostedService>();

            /* Query */

            services.AddScoped<IEventConsumer, EventConsumer>();

            services.AddHostedService<ConsumerHostedService>();


            return services;
        }

    }
}