using Daikon.ApiHost;
using Target.Application;
using Target.Application.Features.Command.NewTarget;
using Target.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(typeof(NewTargetCommandValidator).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
