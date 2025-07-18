
using Aggregators.Application.Disclosure.Dashboard;
using Aggregators.Application.Mappings;
using Daikon.Shared.APIClients.Horizon;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.APIClients.Screen;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Aggregators.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(GenerateDashQuery).Assembly);
            
            // services.AddValidatorsFromAssembly(typeof(RegisterMoleculeValidator).Assembly);
            services.AddScoped<IMLogixAPI, MLogixAPI>();
            services.AddHttpClient<IHorizonAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["HorizonAPI:Url"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddScoped<IHorizonAPI, HorizonAPI>();

            services.AddHttpClient<IScreenAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["ScreenAPI:Url"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddScoped<IScreenAPI, ScreenAPI>();
            services.AddHttpContextAccessor();

            return services;
        }

    }
}