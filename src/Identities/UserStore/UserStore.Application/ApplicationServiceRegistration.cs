
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserStore.Application.Mappings;
using UserStore.Application.Features.Commands.Users.AddUser;
using MediatR;
using FluentValidation;

namespace UserStore.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMediatR(typeof(AddUserCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(AddUserValidator).Assembly);

            return services;
        }
    }
}