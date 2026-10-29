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
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Daikon.ApiHost;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaikonApiDefaults(this IServiceCollection services, IConfiguration configuration, Assembly? validatorsAssembly = null)
    {
        // Every endpoint requires a valid bearer token unless it opts out with AllowAnonymous (only /health does).
        services.AddDaikonAuthentication(configuration);
        services.AddAuthorization(options => options.FallbackPolicy = options.DefaultPolicy);
        services.AddMemoryCache();
        services.AddHttpClient();
        services.TryAddScoped<IAppUserResolver, UserStoreAppUserResolver>();
        services.Configure<KestrelServerOptions>(options => options.AddServerHeader = false);

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
        services.AddSwaggerGen(c =>
        {
            // whatever you already configure (docs, security, xml comments, etc.)

            c.CustomSchemaIds(type =>
            {
                var fullName = type.FullName ?? type.Name;
                return fullName.Replace("+", "."); // nested types
            });
        });


        services.AddHttpContextAccessor();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestorIdBehavior<,>));

        return services;
    }
}
