using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGw.AuthFlowMiddlewares;
using OcelotApiGw.Contracts.Infrastructure;
using OcelotApiGw.OAuth2Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure app configuration
builder.Configuration.AddJsonFile
  ($"OcelotGlobalConfig.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
  .AddJsonFile($"Gateways/{builder.Environment.EnvironmentName}/gw.genes.json", optional: false, reloadOnChange: true)
  .AddJsonFile($"Gateways/{builder.Environment.EnvironmentName}/gw.targets.json", optional: false, reloadOnChange: true);

  
// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

OAuth2Providers.ConfigureEntraIDAuthenticationServices(builder.Services, builder.Configuration);

// Add UserStoreAPIService
builder.Services.AddScoped<IUserStoreAPIService, UserStoreAPIService>();

// Add Ocelot services

builder.Services.AddOcelot();

builder.Services.AddScoped<OAuth2UserAccessHandler>();



var app = builder.Build();

app.UseAuthentication(); 
app.UseAuthorization();

var oAuth2UserAccessHandler = app.Services.GetRequiredService<OAuth2UserAccessHandler>();

var ocelotPipelineConfig = new OcelotPipelineConfiguration
{
    AuthorizationMiddleware = async (ctx, next) =>
    {
        // Invoke custom middleware logic here
        await oAuth2UserAccessHandler.ValidateUser(ctx, next);
    }
};




app.MapGet("/", () => "Hello World!");

app.UseOcelot(ocelotPipelineConfig).Wait();
//app.UseMiddleware<LogAuthHeadersMiddleware>();
app.Run();
