using Comment.Application.EventHandlers;
using Comment.Application.Features.Commands.NewComment;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Comment.Application.Mappings;

namespace Comment.Application;

public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(NewCommentCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(NewCommentCommandValidator).Assembly);

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.UnhandledExceptionBehaviour<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviours.ValidationBehaviour<,>));

            services.AddScoped<ICommentEventHandler, CommentEventHandler>();

            return services;
        }

    }
