
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using HitAssessment.Application.Features.Commands.NewHitAssessment;
using HitAssessment.Application.Mappings;
using HitAssessment.Application.EventHandlers;

namespace HitAssessment.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(NewHitAssessmentCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(NewHitAssessmentCommandValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<IHitAssessmentEventHandler, HitAssessmentEventHandler>();

            return services;
        }

    }
}