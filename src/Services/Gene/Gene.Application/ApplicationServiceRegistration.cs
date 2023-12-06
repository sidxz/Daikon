using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Handlers;
using FluentValidation;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Mappings;
using Gene.Application.Query.Handlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Gene.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(NewGeneCommandHandler).Assembly);
            services.AddValidatorsFromAssembly(typeof(NewGeneCommandValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<IGeneEventHandler, GeneEventHandler>();
            services.AddScoped<IStrainEventHandler, StrainEventHandler>();

            return services;
        }

    }
}