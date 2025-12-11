using Daikon.ApiHost;
using UserPreferences.Application;
using UserPreferences.Application.Features.Commands.TableDefaults;
using UserPreferences.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(typeof(SetTableDefaultsValidator).Assembly);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
