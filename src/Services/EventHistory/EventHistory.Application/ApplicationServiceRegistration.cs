
using EventHistory.Application.BackgroundServices;
using EventHistory.Application.Features.Processors;
using EventHistory.Application.Features.Queries.GetEventHistory;
using EventHistory.Application.Mappers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace EventHistory.Application
{
    /* Dependency Injection for all services */
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(GetEventHistoryQuery).Assembly);
            services.AddScoped<EventMessageProcessor>();
            services.AddHostedService<DefaultEventHistoryPrepBackgroundService>();


            //services.AddValidatorsFromAssembly(typeof(NewProjectCommandValidator).Assembly);
            return services;
        }

    }
}