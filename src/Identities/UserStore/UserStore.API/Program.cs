using Daikon.ApiHost;
using UserStore.API;
using UserStore.Application;
using UserStore.Application.Features.Commands.Users.AddUser;
using UserStore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults(builder.Configuration, typeof(AddUserValidator).Assembly);
builder.Services.AddScoped<IAppUserResolver, LocalAppUserResolver>();
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

app.UseDaikonApiDefaults();

app.Run();
