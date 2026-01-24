using System.Reflection;
using Confluent.Kafka;
using Daikon.ApiHost;
using Daikon.EventManagement.Services;
using Daikon.Events.Gene;
using Daikon.EventStore.Event;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaikonApiDefaults();

builder.Services.AddSingleton<ReplayStatusTracker>();
builder.Services.AddScoped<EventReplayService>();
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddSingleton<IEventProducer, EventProducer>();

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

var eventDbSettings = new EventDatabaseSettings
{
    ConnectionString = builder.Configuration["EventDatabaseSettings:ConnectionString"]
        ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString)),
    DatabaseName = builder.Configuration["EventDatabaseSettings:DatabaseName"]
        ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName))
};
builder.Services.AddSingleton<IEventDatabaseSettings>(eventDbSettings);

var kafkaSettings = new KafkaProducerSettings
{
    BootstrapServers = builder.Configuration["KafkaProducerSettings:BootstrapServers"]
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers)),
    Topic = builder.Configuration["KafkaProducerSettings:Topic"]
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic)),
    SaslMechanism = SaslMechanism.Plain,
    SaslUsername = string.Empty,
    SaslPassword = builder.Configuration["KafkaProducerSettings:ConnectionString"]
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.SaslPassword)),
    SecurityProtocol = Enum.Parse<SecurityProtocol>(
        builder.Configuration["KafkaProducerSettings:SecurityProtocol"] ?? "Plaintext", true)
};
builder.Services.AddSingleton<IKafkaProducerSettings>(kafkaSettings);

builder.Services
    .AddHealthChecks()
    .AddMongoDb(eventDbSettings.ConnectionString, name: "MongoDB")
    .AddKafka(new ProducerConfig { BootstrapServers = kafkaSettings.BootstrapServers }, name: "Kafka");

var app = builder.Build();

app.UseDaikonApiDefaults();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

app.Run();
