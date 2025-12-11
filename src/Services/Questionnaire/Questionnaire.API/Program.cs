using Daikon.ApiHost;
using Questionnaire.Application;
using Questionnaire.Application.Features.Commands.Create;
using Questionnaire.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(typeof(CreateValidator).Assembly);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
