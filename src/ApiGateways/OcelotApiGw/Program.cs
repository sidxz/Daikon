using Ocelot.DependencyInjection;
using Ocelot.Middleware;

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

// Add Ocelot services
builder.Services.AddOcelot();



var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await app.UseOcelot();
app.Run();
