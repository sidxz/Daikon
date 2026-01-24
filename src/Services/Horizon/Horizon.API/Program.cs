using Daikon.ApiHost;
using Horizon.Application;
using Horizon.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
