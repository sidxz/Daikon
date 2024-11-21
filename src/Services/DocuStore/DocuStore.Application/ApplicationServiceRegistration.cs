
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using DocuStore.Application.Mappings;
using DocuStore.Application.Features.Commands.AddParsedDoc;

namespace DocuStore.Application;

public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(AddParsedDocCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(AddParsedDocValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            //services.AddScoped<IDocuStoreEventHandler, DocuStoreEventHandler>();

            return services;
        }

    }
