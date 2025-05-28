using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Daikon.EventManagement.Helper
{
    /*
     * ConfigureSwaggerOptions
     * -----------------------
     * Dynamically generates Swagger documentation for all available API versions.
     * Uses IApiVersionDescriptionProvider to retrieve registered API versions and
     * adds a SwaggerDoc for each, enabling version-specific Swagger UI.
     */
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /*
         * Configures Swagger options globally for all versions.
         */
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
            }
        }

        /*
         * Required by IConfigureNamedOptions — calls the main Configure method.
         */
        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        /*
         * Creates OpenAPI metadata for a specific API version.
         */
        private OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
        {
            var info = new OpenApiInfo
            {
                Title = "Daikon Extensions EventManagement.API",
                Version = desc.ApiVersion.ToString(),
                Description = "API for managing and replaying event store data."
            };

            if (desc.IsDeprecated)
            {
                info.Description += " This API version of Daikon has been deprecated. Please use a newer version.";
            }

            return info;
        }
    }
}
