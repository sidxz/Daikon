
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Screen.Application.BackgroundServices;
using Screen.Application.EventHandlers;
using Screen.Application.Features.Commands.NewScreen;
using Screen.Application.Mappings;

namespace Screen.Application
{
    /* Dependency Injection for all services in Ordering.Application */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(NewScreenCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(NewScreenCommandValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<IScreenEventHandler, ScreenEventHandler>();
            services.AddScoped<IHitCollectionEventHandler, HitCollectionEventHandler>();

            services.AddSingleton<HitBackgroundService>();
            services.AddHostedService<HitBackgroundService>();
            


            return services;
        }

    }
}