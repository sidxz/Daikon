using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Questionnaire.Application.Contracts.Persistence;
using Questionnaire.Infrastructure.Repositories;

namespace Questionnaire.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
             services.AddScoped<IQuestionnaireRepository, QuestionnaireRepository>();
            return services;
        }
    }
}