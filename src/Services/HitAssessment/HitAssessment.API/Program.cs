using Daikon.ApiHost;
using HitAssessment.Application;
using HitAssessment.Application.Features.Commands.NewHitAssessment;
using HitAssessment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(typeof(NewHitAssessmentCommandValidator).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
