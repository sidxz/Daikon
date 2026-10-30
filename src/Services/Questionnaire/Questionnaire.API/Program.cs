using Daikon.ApiHost;
using Questionnaire.Application;
using Questionnaire.Application.Features.Commands.CreateQuestionnaire;
using Questionnaire.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(builder.Configuration, typeof(CreateValidator).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
