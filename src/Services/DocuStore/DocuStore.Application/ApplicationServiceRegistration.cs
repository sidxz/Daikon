// using DocuStore.Application.EventHandlers;
// using DocuStore.Application.Features.Commands.NewDocuStore;
// using FluentValidation;
// using MediatR;
// using Microsoft.Extensions.DependencyInjection;
// using DocuStore.Application.Mappings;

// namespace DocuStore.Application;

// public static class ApplicationServiceRegistration
//     {
//         public static IServiceCollection AddApplicationServices(this IServiceCollection services)
//         {

//             services.AddAutoMapper(typeof(MappingProfile).Assembly);
//             services.AddMediatR(typeof(NewDocuStoreCommand).Assembly);
//             services.AddValidatorsFromAssembly(typeof(NewDocuStoreCommandValidator).Assembly);

//             // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
//             // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

//             services.AddScoped<IDocuStoreEventHandler, DocuStoreEventHandler>();

//             return services;
//         }

//     }
