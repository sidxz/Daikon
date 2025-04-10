using System.Reflection;
using Confluent.Kafka;
using Daikon.EventManagement.Services;
using Daikon.Events.Gene;
using Daikon.EventStore.Event;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register services
builder.Services.AddSingleton<ReplayStatusTracker>();
builder.Services.AddScoped<EventReplayService>();



/* MongoDB Conventions */
var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);
// Register BSON Class Maps

BsonClassMap.RegisterClassMap<BaseEvent>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIsRootClass(true);
                classMap.SetDiscriminatorIsRequired(true);
                classMap.SetIgnoreExtraElements(true);
            });
var assembly = Assembly.GetAssembly(typeof(GeneCreatedEvent));
if (assembly == null)
    throw new InvalidOperationException("Could not find the assembly for BaseEvent");

// Get all types in the Daikon.Events namespace that inherit from BaseEvent
var eventTypes = assembly.GetTypes()
     .Where(t => t.Namespace != null && t.Namespace.StartsWith("Daikon.Events") && t.IsSubclassOf(typeof(BaseEvent))
                 && !t.IsAbstract) // Exclude abstract types (e.g., BaseEvent)
    .ToList();

// Dynamically register each concrete event type
foreach (var eventType in eventTypes)
{
    Console.WriteLine($"Found class map {eventType.Name}");
    if (!BsonClassMap.IsClassMapRegistered(eventType))
    {
        Console.WriteLine($"Registering class map for {eventType.Name}");
        BsonClassMap.LookupClassMap(eventType);
    }
}


var eventDatabaseSettings = new EventDatabaseSettings
{
    ConnectionString = builder.Configuration.GetValue<string>("EventDatabaseSettings:ConnectionString")
                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString), "Event Database connection string is required."),
    DatabaseName = builder.Configuration.GetValue<string>("EventDatabaseSettings:DatabaseName")
                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName), "Event Database name is required."),

};

builder.Services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();


// Configure Kafka Producer
var kafkaProducerSettings = new KafkaProducerSettings
{
    BootstrapServers = builder.Configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers")
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.BootstrapServers), "Kafka BootstrapServers is required."),
    Topic = builder.Configuration.GetValue<string>("KafkaProducerSettings:Topic")
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings.Topic), "Kafka Topic is required."),
    SaslMechanism = SaslMechanism.Plain,
    SaslUsername = "",
    SaslPassword = builder.Configuration.GetValue<string>("KafkaProducerSettings:ConnectionString")
        ?? throw new ArgumentNullException(nameof(KafkaProducerSettings), "Kafka Connection String is required."),
    SecurityProtocol = Enum.Parse<SecurityProtocol>(
        builder.Configuration.GetValue<string>("KafkaProducerSettings:SecurityProtocol") ?? "Plaintext", true),
};


builder.Services.AddSingleton<IKafkaProducerSettings>(kafkaProducerSettings);
builder.Services.AddSingleton<IEventProducer, EventProducer>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

//app.UseHttpsRedirection();



app.Run();
