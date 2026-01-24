using CQRS.Core.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Daikon.ApiHost;

public static class WebApplicationExtensions
{
    private static readonly DateTimeOffset StartedAt = DateTimeOffset.UtcNow;

    public static WebApplication UseDaikonApiDefaults(this WebApplication app)
    {
        Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

        if (app.Environment.IsDevelopment())
        {
            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
        }

        app.MapGet("/health", () => Results.Ok(BuildHealthResponse(app)));
        app.MapGet("/health/ready", () => Results.Ok(BuildHealthResponse(app)));
        app.UseMiddleware<GlobalErrorHandlingMiddleware>();
        app.MapControllers();

        return app;
    }

    private static IDictionary<string, object?> BuildHealthResponse(WebApplication app)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var serviceName = app.Environment.ApplicationName ?? entryAssembly?.GetName().Name;
        var version = entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? entryAssembly?.GetName().Version?.ToString();
        var response = new Dictionary<string, object?>
        {
            ["service"] = serviceName,
            ["version"] = version,
            ["environment"] = app.Environment.EnvironmentName,
            ["timestamp"] = DateTimeOffset.UtcNow,
            ["uptime"] = DateTimeOffset.UtcNow - StartedAt
        };

        var commit = app.Configuration["Build:Commit"];
        if (!string.IsNullOrWhiteSpace(commit))
        {
            response["commit"] = commit;
        }

        var buildNumber = app.Configuration["Build:BuildNumber"];
        if (!string.IsNullOrWhiteSpace(buildNumber))
        {
            response["buildNumber"] = buildNumber;
        }

        return response;
    }
}
