using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Gene.Application;
using Gene.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// builder.Services.AddApiVersioning(
//     options =>
//     {
//         // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
//         options.ReportApiVersions = true;
//         options.DefaultApiVersion = new ApiVersion(2, 0);
//         options.AssumeDefaultVersionWhenUnspecified = true;
//         options.ApiVersionReader = new UrlSegmentApiVersionReader();
//     });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application and Infrastructure services.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService(builder.Configuration);


var app = builder.Build();

app.MapControllers();

// Print the environment name to the console.
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();



app.Run();
