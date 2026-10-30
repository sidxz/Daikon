using Daikon.ApiHost;
using Aggregators.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
