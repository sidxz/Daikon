using System.Reflection;
using CQRS.Core.Middlewares;
using Daikon.ApiHost.Routing;
using Daikon.ApiHost.Swagger;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Daikon.ApiHost;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaikonApiDefaults(this IServiceCollection services, Assembly? validatorsAssembly = null)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                            new HeaderApiVersionReader("x-api-version"),
                                                            new MediaTypeApiVersionReader("x-api-version"));
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        });

        var fluentValidationBuilder = services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        if (validatorsAssembly is not null)
        {
            fluentValidationBuilder.AddValidatorsFromAssembly(validatorsAssembly);
        }

        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen();

        services.AddHttpContextAccessor();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestorIdBehavior<,>));

        return services;
    }
}
