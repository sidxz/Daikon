using Daikon.ApiHost;
using DocuStore.Application;
using DocuStore.Application.Features.Commands.AddParsedDoc;
using DocuStore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(builder.Configuration, typeof(AddParsedDocValidator).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
