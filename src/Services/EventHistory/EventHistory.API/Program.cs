using CQRS.Core.Middlewares;
using EventHistory.API.Conventions;
using EventHistory.API.Helper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using EventHistory.Infrastructure;
using EventHistory.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// API Versioning setup for supporting versioned APIs
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    // Combining multiple API version readers (via URL, header, and media type)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));
});

// Setup for versioned API explorer with proper version substitution in URLs
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";  // Format for API version groups
    options.SubstituteApiVersionInUrl = true;  // Substitute version in route URLs
});

// Enable lowercase URLs for better API readability
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Adding controllers and slugified route convention for better URL readability
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

// Add Swagger and OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

// Register application and infrastructure services (uncomment if services are needed)

builder.Services.AddApplicationServices();  // Application layer services
builder.Services.AddInfrastructureService(builder.Configuration);  // Infrastructure services

// HttpContext accessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

// Inject User Id into Command and Query pipeline via custom pipeline behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestorIdBehavior<,>));

// Build the WebApplication
var app = builder.Build();

// Log the environment name to the console for visibility
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

// Configure the HTTP request pipeline based on the environment
if (app.Environment.IsDevelopment())
{
    // Swagger UI setup for versioned APIs
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

// Add global error handling middleware to catch and process exceptions
app.UseMiddleware<GlobalErrorHandlingMiddleware>();

// Map controllers to routes
app.MapControllers();

// Optional: Uncomment to enforce HTTPS redirection in production environments
// app.UseHttpsRedirection();

app.Run();
