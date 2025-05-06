
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserPreferences.Application.Contracts.Persistence;
using UserPreferences.Infrastructure.Repositories;

namespace UserPreferences.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITableDefaultsRepository, TableDefaultsRepository>();
            services.AddScoped<ITableGlobalConfigRepository, TableGlobalConfigRepository>();
            services.AddScoped<ITableUserCustomizationRepository, TableUserCustomizationRepository>();
            return services;
        }
    }
}