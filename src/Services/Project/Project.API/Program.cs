using Daikon.ApiHost;
using Project.Application;
using Project.Application.Features.Commands.NewProject;
using Project.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(typeof(NewProjectCommandValidator).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
