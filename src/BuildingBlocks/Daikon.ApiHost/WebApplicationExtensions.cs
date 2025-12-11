using CQRS.Core.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Daikon.ApiHost;

public static class WebApplicationExtensions
{
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

        app.UseMiddleware<GlobalErrorHandlingMiddleware>();
        app.MapControllers();

        return app;
    }
}
