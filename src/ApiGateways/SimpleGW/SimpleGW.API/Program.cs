
using Microsoft.IdentityModel.Logging;
using SimpleGW.API.Middlewares;
using SimpleGW.Contracts.Infrastructure;
using SimpleGW.OIDCProviders;

var builder = WebApplication.CreateBuilder(args);
var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();

// Disable logging of PII (Personally Identifiable Information)
IdentityModelEventSource.ShowPII = false;
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
builder.Logging.AddFilter("IdentityModelEventSource.ShowPII", LogLevel.Warning);


try
{
    // Select the OIDC provider based on the settings
    var authProvider = builder.Configuration["OIDCProvider"];
    if (string.IsNullOrWhiteSpace(authProvider))
    {
        throw new InvalidOperationException("OIDCProvider configuration is missing.");
    }

    switch (authProvider)
    {
        case "KeyCloak":
            KeyCloak.Configure(builder.Services, builder.Configuration);
            break;
        case "EntraID":
            MicrosoftEntraID.Configure(builder.Services, builder.Configuration, logger);
            break;
        default:
            throw new InvalidOperationException($"Unrecognized OIDCProvider configuration: {authProvider}.");
    }

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient();
    builder.Services.AddMemoryCache();


    builder.Services.AddScoped<IUserStoreAPIService, UserStoreAPIService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    });

    var app = builder.Build();

    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors("AllowAll");

    app.UseMiddleware<RequestTimingMiddleware>();
    app.UseMiddleware<AuthenticationValidatorMiddleware>();
    app.UseMiddleware<UserProfileEnricherMiddleware>();
    app.UseMiddleware<MicroServiceRoutingMiddleware>();
    app.UseMiddleware<RequestForwardingMiddleware>();

    app.Run();
}
catch (Exception ex)
{
    Thread.Sleep(1000); // Wait for the logger to initiate
    logger.LogError(ex, "Application startup failed due to configuration error");
    //Console.Error.WriteLine(ex.Message);
    //Console.WriteLine(ex.Message);
    //Console.Error.WriteLine("Application startup failed due to configuration error");
    //Console.WriteLine("Application startup failed due to configuration error");
    Thread.Sleep(1000);
    Environment.Exit(1); // Non-zero code indicates an error
}
