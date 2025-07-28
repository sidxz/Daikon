
using FluentValidation;
using Horizon.Application.Contracts.Persistence;
using Horizon.Application.Features.Calculation;
using Horizon.Application.Features.Command.Gene.AddGene;
using Horizon.Application.Handlers;
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

           
            services.AddMediatR(typeof(AddGeneCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(AddGeneCommandValidator).Assembly);

            

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));
            services.AddScoped<RootFinder>();
            services.AddScoped<IGeneEventHandler, GeneEventHandler>();
            services.AddScoped<ITargetEventHandler, TargetEventHandler>();
            services.AddScoped<IScreenEventHandler, ScreenEventHandler>();
            services.AddScoped<IHitCollectionEventHandler, HitCollectionEventHandler>();
            services.AddScoped<IHitAssessmentEventHandler, HitAssessmentEventHandler>();
            services.AddScoped<IProjectEventHandler, ProjectEventHandler>();
            services.AddScoped<IMLogixEventHandler, MLogixEventHandler>();
            services.AddHostedService<RemoveDuplicateMoleculeRelationsBackgroundService>();


            return services;
        }
        
    }
}