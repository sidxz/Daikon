using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserStore.Application.Contracts.Persistence;
using UserStore.Infrastructure.Repositories;

namespace UserStore.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAppOrgRepository, AppOrgRepository>();
            services.AddScoped<IAppRoleRepository, AppRoleRepository>();
            services.AddScoped<IAPIResourceRepository, APIResourceRepository>();
            
            return services;
        }
    }
}