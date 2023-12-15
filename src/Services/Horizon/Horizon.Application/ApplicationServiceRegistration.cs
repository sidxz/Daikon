
using FluentValidation;
using Horizon.Application.Contracts.Persistance;
using Horizon.Application.Features.Command.Gene.AddGeneToGraph;
using Horizon.Application.Query.Handlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Horizon.Application
{
    /* Dependency Injection for all services in Horizon.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

           
            services.AddMediatR(typeof(AddGeneToGraphCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(AddGeneToGraphCommandValidator).Assembly);

            

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<IGeneEventHandler, GeneEventHandler>();
            services.AddScoped<ITargetEventHandler, TargetEventHandler>();

            return services;
        }
        
    }
}