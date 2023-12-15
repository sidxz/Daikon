
using FluentValidation;

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Target.Application.EventHandlers;
using Target.Application.Features.Command.NewTarget;
using Target.Application.Mappings;

namespace Target.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            
            services.AddMediatR(typeof(NewTargetCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(NewTargetCommandValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<ITargetEventHandler, TargetEventHandler>();
          

            return services;
        }

    }
}