using Daikon.EventStore.Event;
using Daikon.Events.Gene;
using Daikon.EventStore.Settings;
using EventHistory.Application.Contracts.Persistence;
using EventHistory.Infrastructure.Query.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Daikon.Shared.APIClients.UserStore;
using System.Reflection;
using Daikon.Shared.APIClients.HitAssessment;
using Daikon.Shared.APIClients.Screen;
using Daikon.Shared.APIClients.Project;
using Daikon.Shared.APIClients.Gene;
using Daikon.Shared.Embedded.Screens;
using Daikon.Shared.Constants.InternalSettings;
namespace EventHistory.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddHttpClient<IUserStoreAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["UserStoreAPI:Url"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = Timeouts.HttpClientTimeout;
            });


            services.AddHttpClient<IGeneAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["GeneAPI:Url"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = Timeouts.HttpClientTimeout;
            });


            services.AddHttpClient<IScreenAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["ScreenAPI:Url"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = Timeouts.HttpClientTimeout;
            });

            services.AddHttpClient<IHitAssessmentAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["HitAssessmentAPI:Url"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = Timeouts.HttpClientTimeout;
            });

            services.AddHttpClient<IProjectAPI>(client =>
            {
                // Assuming the API base URL is stored in configuration
                client.BaseAddress = new Uri(configuration["ProjectAPI:Url"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = Timeouts.HttpClientTimeout;
            });



            /* MongoDB Conventions */
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsGlobally", conventionPack, t => true);

            //BsonSerializer.RegisterSerializationProvider(new DVariableSerializationProvider());

            RegisterBsonClassMaps();

            /* Event Database */
            var eventDatabaseSettings = GetEventDatabaseSettings(configuration);
            services.AddSingleton<IEventDatabaseSettings>(eventDatabaseSettings);
            services.AddScoped<IEventStoreRepositoryExtension, EventStoreRepositoryExtension>();

            services.AddScoped<IUserStoreAPI, UserStoreAPI>();
            services.AddScoped<IGeneAPI, GeneAPI>();
            services.AddScoped<IScreenAPI, ScreenAPI>();
            services.AddScoped<IHitAssessmentAPI, HitAssessmentAPI>();
            services.AddScoped<IProjectAPI, ProjectAPI>();
            return services;
        }


        private static EventDatabaseSettings GetEventDatabaseSettings(IConfiguration configuration)
        {
            return new EventDatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("EventDatabaseSettings:ConnectionString")
                                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.ConnectionString), "Event Database connection string is required."),
                DatabaseName = configuration.GetValue<string>("EventDatabaseSettings:DatabaseName")
                                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.DatabaseName), "Event Database name is required."),
            };
        }


        private static void RegisterBsonClassMaps()
        {
            //BsonClassMap.RegisterClassMap<DocMetadata>();
            //BsonClassMap.RegisterClassMap<EventModel>();
            Console.WriteLine("Registering class maps for all events");
            BsonClassMap.RegisterClassMap<BaseEvent>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIsRootClass(true);
                classMap.SetDiscriminatorIsRequired(true);
                classMap.SetIgnoreExtraElements(true);
            });

            //BsonClassMap.RegisterClassMap<HaCreatedEvent>();

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


        }

    }
}
