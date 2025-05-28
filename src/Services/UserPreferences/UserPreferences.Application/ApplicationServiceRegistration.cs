
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UserPreferences.Application.Features.Commands.SetTableDefaults;
using UserPreferences.Application.Mappings;


namespace UserPreferences.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            
            services.AddMediatR(typeof(SetTableDefaultsCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(SetTableDefaultsValidator).Assembly);
            return services;
        }

    }
}