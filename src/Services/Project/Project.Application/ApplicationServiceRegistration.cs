
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Features.Commands.NewProject;
using Project.Application.Mappings;
using Project.Application.EventHandlers;

namespace Project.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(NewProjectCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(NewProjectCommandValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<IProjectEventHandler, ProjectEventHandler>();

            return services;
        }

    }
}