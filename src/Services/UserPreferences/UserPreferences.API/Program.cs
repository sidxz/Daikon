using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using CQRS.Core.Middlewares;
using MediatR;
using UserPreferences.API.Conventions;
using UserPreferences.Application.Features.Commands.SetTableDefaults;
using UserPreferences.API.Helper;
using UserPreferences.Application;
using UserPreferences.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// API VERSIONING
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
});
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddLogging();

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(
            new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    });



builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<SetTableDefaultsValidator>();


// Add Swagger and OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

// Register application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);


// HttpContext accessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

// Inject User Id in Command and Query
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestorIdBehavior<,>));


var app = builder.Build();

// Print the environment name to the console.
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
// Configure the HTTP request pipeline.
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

// Add the custom global error handling middleware
app.UseMiddleware<GlobalErrorHandlingMiddleware>();

// Map Controllers
app.MapControllers();

app.Run();