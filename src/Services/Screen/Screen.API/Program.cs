using Daikon.ApiHost;
using Screen.Application;
using Screen.Application.Features.Command.NewScreen;
using Screen.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(typeof(NewScreenCommandValidator).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
