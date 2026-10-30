
using Microsoft.IdentityModel.Logging;
using SimpleGW.API.Middlewares;
using SimpleGW.API.Services;
using CQRS.Core.Middlewares;
using Daikon.ApiHost;

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
    // Same token validation as every downstream service (OIDCProvider selects EntraID or KeyCloak)
    builder.Services.AddDaikonAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();
    // The token's owner must also be a DAIKON user, resolved through UserStore and cached per subject, as every service does
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<IAppUserResolver, UserStoreAppUserResolver>();
    if (string.IsNullOrWhiteSpace(builder.Configuration["UserStoreAPI:Url"]))
    {
        throw new InvalidOperationException("UserStoreAPI:Url configuration is missing or empty.");
    }
    builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient("SimpleGWClient")
        .ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromMinutes(5);
        });
    builder.Services.AddSingleton<IMicroserviceHealthStore, MicroserviceHealthStore>();
    builder.Services.AddHostedService<MicroserviceHealthPoller>();

    // Only the configured frontend origin(s) may call the API from a browser, e.g. "https://daikon.example.org,http://localhost:3000"
    var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (allowedOrigins is not { Length: > 0 })
    {
        throw new InvalidOperationException("Cors:AllowedOrigins configuration is missing or empty.");
    }
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy => policy
            .WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders("Authorization", "Content-Type", "x-api-version"));
    });

    var app = builder.Build();

    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

    // No-store and security headers on every response, including CORS preflights and errors
    app.UseMiddleware<SecurityHeadersMiddleware>();
    // A failure while resolving the user (UserStore unreachable) is a 503 with a generic body, never a raw 500
    app.UseMiddleware<GlobalErrorHandlingMiddleware>();

    app.UseRouting();
    app.UseCors("Frontend");
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<RequestTimingMiddleware>();
    app.UseMiddleware<AuthenticationValidatorMiddleware>();
    app.UseMiddleware<AppUserMiddleware>(); // valid token but not a DAIKON user: 401 here, nothing is forwarded

    // Signed-in users only: it lists internal services, versions and errors
    app.Map("/health/services", healthApp =>
    {
        healthApp.Run(async context =>
        {
            var store = context.RequestServices.GetRequiredService<IMicroserviceHealthStore>();
            var snapshot = store.GetSnapshot();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(snapshot, context.RequestAborted);
        });
    });

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
