
using Horizon.Application.Contracts.Persistance;
using Horizon.Infrastructure.HostedServices;
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

            services.AddScoped<IGraphRepository, GraphRepository>();

            string neo4jUri = configuration.GetValue<string>("HorizonNeo4jSettings:Uri") ?? throw new ArgumentNullException(nameof(neo4jUri));
            string neo4jUser = configuration.GetValue<string>("HorizonNeo4jSettings:User") ?? throw new ArgumentNullException(nameof(neo4jUser));
            string neo4jPassword = configuration.GetValue<string>("HorizonNeo4jSettings:Password") ?? throw new ArgumentNullException(nameof(neo4jPassword));


            services.AddSingleton(GraphDatabase.Driver(
                neo4jUri,
                AuthTokens.Basic(
                    neo4jUser,
                    neo4jPassword
                )
            ));

            services.AddHostedService<Neo4jDatabaseSetupHostedService>();

            /* Query */


            return services;
        }

    }
}