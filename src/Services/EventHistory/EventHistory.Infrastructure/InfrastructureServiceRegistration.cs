using CQRS.Core.Domain;
using CQRS.Core.Event;
using Daikon.Events.Comment;
using Daikon.Events.Gene;
using Daikon.Events.HitAssessment;
using Daikon.Events.MLogix;
using Daikon.Events.Project;
using Daikon.Events.Strains;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;
using EventHistory.Application.Contracts.Persistence;
using EventHistory.Application.Features.Queries.GetEventHistory;
using EventHistory.Infrastructure.Query.Repositories;
using EventHistory.Infrastructure.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http;
using Daikon.Shared.APIClients.UserStore;
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
                CollectionName = configuration.GetValue<string>("EventDatabaseSettings:CollectionName")
                                    ?? throw new ArgumentNullException(nameof(EventDatabaseSettings.CollectionName), "Event Database collection name is required.")
            };
        }


        private static void RegisterBsonClassMaps()
        {
            //BsonClassMap.RegisterClassMap<DocMetadata>();
            //BsonClassMap.RegisterClassMap<EventModel>();
            BsonClassMap.RegisterClassMap<BaseEvent>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIsRootClass(true);
                classMap.SetDiscriminatorIsRequired(true);
            });
            BsonClassMap.RegisterClassMap<HaCreatedEvent>();
            //             BsonClassMap.RegisterClassMap<HaCreatedEvent>(cm =>
            // {
            //     cm.AutoMap();
            //     cm.SetDiscriminator(nameof(HaCreatedEvent)); // Ensure the correct discriminator is used
            // });
            BsonClassMap.RegisterClassMap<HaUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HaDeletedEvent>();
            BsonClassMap.RegisterClassMap<HaRenamedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionAddedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<HaCompoundEvolutionDeletedEvent>();
            BsonClassMap.RegisterClassMap<CommentCreatedEvent>();
            BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
            BsonClassMap.RegisterClassMap<CommentDeletedEvent>();
            BsonClassMap.RegisterClassMap<CommentReplyAddedEvent>();
            BsonClassMap.RegisterClassMap<CommentReplyUpdatedEvent>();
            BsonClassMap.RegisterClassMap<CommentReplyDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneCreatedEvent>();
            BsonClassMap.RegisterClassMap<GeneUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneDeletedEvent>();
            BsonClassMap.RegisterClassMap<StrainCreatedEvent>();
            BsonClassMap.RegisterClassMap<StrainUpdatedEvent>();
            BsonClassMap.RegisterClassMap<StrainDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneEssentialityAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneEssentialityUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneEssentialityDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinProductionAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinProductionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinProductionDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinActivityAssayAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinActivityAssayUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneProteinActivityAssayDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneHypomorphAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneHypomorphUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneHypomorphDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneCrispriStrainAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneCrispriStrainUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneCrispriStrainDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneResistanceMutationAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneResistanceMutationUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneResistanceMutationDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneVulnerabilityAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneVulnerabilityUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneVulnerabilityDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneUnpublishedStructuralInformationAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneUnpublishedStructuralInformationUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneUnpublishedStructuralInformationDeletedEvent>();
            BsonClassMap.RegisterClassMap<GeneExpansionPropAddedEvent>();
            BsonClassMap.RegisterClassMap<GeneExpansionPropUpdatedEvent>();
            BsonClassMap.RegisterClassMap<GeneExpansionPropDeletedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeCreatedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeUpdatedEvent>();
            BsonClassMap.RegisterClassMap<MoleculeDeletedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCreatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectDeletedEvent>();
            BsonClassMap.RegisterClassMap<ProjectRenamedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionAddedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionUpdatedEvent>();
            BsonClassMap.RegisterClassMap<ProjectCompoundEvolutionDeletedEvent>();
            BsonClassMap.RegisterClassMap<ProjectAssociationUpdatedEvent>();
        }

    }
}
