
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MLogix.Application.Mappings;
using MLogix.Application.EventHandlers;
using MLogix.Application.Features.Commands.RegisterMolecule;

namespace MLogix.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(RegisterMoleculeCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(RegisterMoleculeValidator).Assembly);
            services.AddHttpContextAccessor();

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<IMLogixEventHandler, MLogixEventHandler>();

            return services;
        }

    }
}