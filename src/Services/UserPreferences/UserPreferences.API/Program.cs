using Daikon.ApiHost;
using UserPreferences.Application;
using UserPreferences.Application.Features.Commands.SetTableDefaults;
using UserPreferences.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(typeof(SetTableDefaultsValidator).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
