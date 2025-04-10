using System.Reflection;
using Confluent.Kafka;
using CQRS.Core.Middlewares;
using Daikon.EventManagement.Conventions;
using Daikon.EventManagement.Helper;
using Daikon.EventManagement.Services;
using Daikon.Events.Gene;
using Daikon.EventStore.Event;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

/* ================================================
 * 🔧 API Versioning & Routing
 * ================================================ */
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

/* ================================================
 * 📚 Swagger & API Explorer
 * ================================================ */
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

/* ================================================
 * 🌱 Core Application Services
 * ================================================ */
builder.Services.AddSingleton<ReplayStatusTracker>();
builder.Services.AddScoped<EventReplayService>();
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddSingleton<IEventProducer, EventProducer>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestorIdBehavior<,>));

/* ================================================
 * 🍃 MongoDB Setup
 * ================================================ */
var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, _ => true);

BsonClassMap.RegisterClassMap<BaseEvent>(cm =>
{
    cm.AutoMap();
    cm.SetIsRootClass(true);
    cm.SetDiscriminatorIsRequired(true);
    cm.SetIgnoreExtraElements(true);
});

var eventAssembly = Assembly.GetAssembly(typeof(GeneCreatedEvent))
    ?? throw new InvalidOperationException("Could not find the assembly for BaseEvent.");

var eventTypes = eventAssembly.GetTypes()
    .Where(t => t.Namespace?.StartsWith("Daikon.Events") == true &&
                t.IsSubclassOf(typeof(BaseEvent)) &&
                !t.IsAbstract)
    .ToList();

foreach (var type in eventTypes)
{
    if (!BsonClassMap.IsClassMapRegistered(type))
    {
        try
        {
            Console.WriteLine($"Registering class map for {type.Name}");
            BsonClassMap.LookupClassMap(type);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"⚠️ Failed to register {type.Name}: {ex.Message}");
        }
    }
}

/* ================================================
 * 🛢 MongoDB Settings Binding
 * ================================================ */
var eventDbSettings = new EventDatabaseSettings
{
    ConnectionString = builder.Configuration["EventDatabaseSettings:ConnectionString"]
        ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
    DatabaseName = builder.Configuration["EventDatabaseSettings:DatabaseName"]
        ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName))
};
builder.Services.AddSingleton<IEventDatabaseSettings>(eventDbSettings);

/* ================================================
 * 🚀 Kafka Producer Settings
 * ================================================ */
var kafkaSettings = new KafkaProducerSettings
{
    BootstrapServers = builder.Configuration["KafkaProducerSettings:BootstrapServers"]
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
    Topic = builder.Configuration["KafkaProducerSettings:Topic"]
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic)),
    SaslMechanism = SaslMechanism.Plain,
    SaslUsername = "",
    SaslPassword = builder.Configuration["KafkaProducerSettings:ConnectionString"]
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.SaslPassword)),
    SecurityProtocol = Enum.Parse<SecurityProtocol>(
        builder.Configuration["KafkaProducerSettings:SecurityProtocol"] ?? "Plaintext", true)
};
builder.Services.AddSingleton<IKafkaProducerSettings>(kafkaSettings);

/* ================================================
 * 🧪 Health Checks
 * ================================================ */
builder.Services
    .AddHealthChecks()
    .AddMongoDb(eventDbSettings.ConnectionString, name: "MongoDB")
    .AddKafka(new ProducerConfig { BootstrapServers = kafkaSettings.BootstrapServers }, name: "Kafka");

/* ================================================
 * 🏁 Application Pipeline
 * ================================================ */
var app = builder.Build();

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    });
}

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

app.Run();
