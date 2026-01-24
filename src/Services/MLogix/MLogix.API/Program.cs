using Daikon.ApiHost;
using MLogix.Application;
using MLogix.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
