using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGw.AuthFlowMiddlewares;
using OcelotApiGw.Contracts.Infrastructure;
using OcelotApiGw.OIDCProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();


// Configure Ocelot configuration.

builder.Configuration.AddJsonFile
  ($"OcelotGlobalConfig.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
  .AddJsonFile($"Gateways/{builder.Environment.EnvironmentName}/gw.genes.json", optional: false, reloadOnChange: true)
  .AddJsonFile($"Gateways/{builder.Environment.EnvironmentName}/gw.targets.json", optional: false, reloadOnChange: true);

// Select the OIDC provider based on the settings
var authProvider = builder.Configuration["OIDCProvider"];
switch (authProvider)
{
  case "KeyCloak":
    KeyCloak.Configure(builder.Services, builder.Configuration);
    break;
  case "EntraID":
    MicrosoftEntraID.Configure(builder.Services, builder.Configuration);
    break;
  default:
    MicrosoftEntraID.Configure(builder.Services, builder.Configuration);
    break;
}


// Register a http client for the User Store API
builder.Services.AddHttpClient<IUserStoreAPIService, UserStoreAPIService>();

// Add UserStoreAPIService
builder.Services.AddScoped<IUserStoreAPIService, UserStoreAPIService>();

// Add Ocelot services
builder.Services.AddOcelot();

// Middlewares
builder.Services.AddScoped<OAuth2UserAccessHandler>();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

var oAuth2UserAccessHandler = app.Services.GetRequiredService<OAuth2UserAccessHandler>();

var ocelotPipelineConfig = new OcelotPipelineConfiguration
{
  AuthorizationMiddleware = async (ctx, next) =>
  {
    await oAuth2UserAccessHandler.ValidateUser(ctx, next);
  }
};

app.MapGet("/", () => "Hello World!");

app.UseOcelot(ocelotPipelineConfig).Wait();

app.Run();
