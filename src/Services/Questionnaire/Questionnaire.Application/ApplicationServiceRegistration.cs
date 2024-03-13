
using FluentValidation;

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Questionnaire.Application.Features.Commands.CreateQuestionnaire;
using Questionnaire.Application.Mappings;

namespace Questionnaire.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            
            services.AddMediatR(typeof(CreateCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));
            
          
            return services;
        }

    }
}