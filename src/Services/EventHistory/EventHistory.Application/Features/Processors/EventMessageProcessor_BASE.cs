
using AutoMapper;
using CQRS.Core.Event;
using Daikon.Events.Gene;
using Daikon.Events.HitAssessment;
using Daikon.Events.Project;
using Daikon.Events.Screens;
using Daikon.Events.Targets;
using Daikon.Shared.APIClients.Gene;
using Daikon.Shared.APIClients.HitAssessment;
using Daikon.Shared.APIClients.Project;
using Daikon.Shared.APIClients.Screen;
using Daikon.Shared.APIClients.UserStore;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private readonly IUserStoreAPI _userStoreAPI;

        private readonly IGeneAPI _geneAPI; 
        private readonly IScreenAPI _screenAPI;
        private readonly IHitAssessmentAPI _hitAssessmentAPI;
        private readonly IProjectAPI _projectAPI;

        private readonly IMapper _mapper;
        private readonly ILogger<EventMessageProcessor> _logger;

        private  bool refreshCache = false;

        public EventMessageProcessor(
            IUserStoreAPI userStoreAPI,
            IGeneAPI geneAPI,
            IScreenAPI screenAPI,
            IHitAssessmentAPI hitAssessmentAPI,
            IProjectAPI projectAPI,
            IMapper mapper,
            ILogger<EventMessageProcessor> logger)
        {
            _userStoreAPI = userStoreAPI ?? throw new ArgumentNullException(nameof(userStoreAPI));
            _geneAPI = geneAPI ?? throw new ArgumentNullException(nameof(geneAPI));
            _screenAPI = screenAPI ?? throw new ArgumentNullException(nameof(screenAPI));
            _hitAssessmentAPI = hitAssessmentAPI ?? throw new ArgumentNullException(nameof(hitAssessmentAPI));
            _projectAPI = projectAPI ?? throw new ArgumentNullException(nameof(projectAPI));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<EventMessageResult> Process(BaseEvent eventData, bool refreshCache = false)
        {
            this.refreshCache = refreshCache;
            try
            {
                return eventData switch
                {
                    /* Gene */
                    GeneCreatedEvent geneCreatedEvent => await HandleGeneCreatedEventAsync(geneCreatedEvent),
                    GeneUpdatedEvent geneUpdatedEvent => await HandleGeneUpdatedEventAsync(geneUpdatedEvent),

                    GeneCrispriStrainAddedEvent geneCrispriStrainAddedEvent => await HandleGeneCrispriStrainAddedEventAsync(geneCrispriStrainAddedEvent),
                    GeneCrispriStrainUpdatedEvent geneCrispriStrainUpdatedEvent => await HandleGeneCrispriStrainUpdatedEventAsync(geneCrispriStrainUpdatedEvent),
                    GeneEssentialityAddedEvent geneEssentialityAddedEvent => await HandleGeneEssentialityAddedEventAsync(geneEssentialityAddedEvent),
                    GeneEssentialityUpdatedEvent geneEssentialityUpdatedEvent => await HandleGeneEssentialityUpdatedEventAsync(geneEssentialityUpdatedEvent),
                    GeneHypomorphAddedEvent geneHypomorphAddedEvent => await HandleGeneHypomorphAddedEventAsync(geneHypomorphAddedEvent),
                    GeneHypomorphUpdatedEvent geneHypomorphUpdatedEvent => await HandleGeneHypomorphUpdatedEventAsync(geneHypomorphUpdatedEvent),
                    GeneProteinActivityAssayAddedEvent geneProteinActivityAssayAddedEvent => await HandleGeneProteinActivityAssayAddedEventAsync(geneProteinActivityAssayAddedEvent),
                    GeneProteinActivityAssayUpdatedEvent geneProteinActivityAssayUpdatedEvent => await HandleGeneProteinActivityAssayUpdatedEventAsync(geneProteinActivityAssayUpdatedEvent),
                    GeneProteinProductionAddedEvent geneProteinProductionAddedEvent => await HandleGeneProteinProductionAddedEventAsync(geneProteinProductionAddedEvent),
                    GeneProteinProductionUpdatedEvent geneProteinProductionUpdatedEvent => await HandleGeneProteinProductionUpdatedEventAsync(geneProteinProductionUpdatedEvent),
                    GeneResistanceMutationAddedEvent geneResistanceMutationAddedEvent => await HandleGeneResistanceMutationAddedEventAsync(geneResistanceMutationAddedEvent),
                    GeneResistanceMutationUpdatedEvent geneResistanceMutationUpdatedEvent => await HandleGeneResistanceMutationUpdatedEventAsync(geneResistanceMutationUpdatedEvent),
                    GeneUnpublishedStructuralInformationAddedEvent geneUnpublishedStructuralInformationAddedEvent => await HandleGeneUnpublishedStructuralInformationAddedEventAsync(geneUnpublishedStructuralInformationAddedEvent),
                    GeneUnpublishedStructuralInformationUpdatedEvent geneUnpublishedStructuralInformationUpdatedEvent => await HandleGeneUnpublishedStructuralInformationUpdatedEventAsync(geneUnpublishedStructuralInformationUpdatedEvent),
                    GeneVulnerabilityAddedEvent geneVulnerabilityAddedEvent => await HandleGeneVulnerabilityAddedEventAsync(geneVulnerabilityAddedEvent),
                    GeneVulnerabilityUpdatedEvent geneVulnerabilityUpdatedEvent => await HandleGeneVulnerabilityUpdatedEventAsync(geneVulnerabilityUpdatedEvent),
                    
                    /* Target */
                    TargetCreatedEvent targetCreatedEvent => await HandleTargetCreatedEventAsync(targetCreatedEvent),
                    TargetUpdatedEvent targetUpdatedEvent => await HandleTargetUpdatedEventAsync(targetUpdatedEvent),

                    /* Screen */
                    ScreenCreatedEvent screenCreatedEvent => await HandleScreenCreatedEventAsync(screenCreatedEvent),
                    ScreenUpdatedEvent screenUpdatedEvent => await HandleScreenUpdatedEventAsync(screenUpdatedEvent),

                    HitCollectionCreatedEvent hitCollectionCreatedEvent => await HandleHitColCreatedEventAsync(hitCollectionCreatedEvent),
                    HitCollectionUpdatedEvent hitCollectionUpdatedEvent => await HandleHitColUpdatedEventAsync(hitCollectionUpdatedEvent),
                    HitAddedEvent hitAddedEvent => await HandleHitAddedEventAsync(hitAddedEvent),
                    /* HIt Assessment */
                    HaCreatedEvent haCreatedEvent => await HandleHaCreatedEvent(haCreatedEvent),
                    HaUpdatedEvent haUpdatedEvent => await HandleHaUpdatedEvent(haUpdatedEvent),
                    HaCompoundEvolutionAddedEvent haCompoundEvolutionAddedEvent => await HandleHACEAddedEvent(haCompoundEvolutionAddedEvent),
                    HaCompoundEvolutionUpdatedEvent haCompoundEvolutionUpdatedEvent => await HandleHACEUpdatedEvent(haCompoundEvolutionUpdatedEvent),
                    /* Project */
                    ProjectCreatedEvent projectCreatedEvent => await HandleProjectCreatedEvent(projectCreatedEvent),
                    ProjectUpdatedEvent projectUpdatedEvent => await HandleProjectUpdatedEvent(projectUpdatedEvent),
                    ProjectCompoundEvolutionAddedEvent projectCompoundEvolutionAddedEvent => await HandleProjectCEAddedEvent(projectCompoundEvolutionAddedEvent),
                    ProjectCompoundEvolutionUpdatedEvent projectCompoundEvolutionUpdatedEvent => await HandleProjectCEUpdatedEvent(projectCompoundEvolutionUpdatedEvent),


                    _ => new EventMessageResult
                    {
                        Message = "Unsupported event",
                        Link = string.Empty
                    },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing event of type {eventData.GetType().Name}");
                return new EventMessageResult
                {
                    Message = "An error occurred while processing the event",
                    Link = string.Empty
                };
            }
        }


        // Retrieves organization name by ID or returns "Unknown" if not found
        protected async Task<string> GetOrganizationNameAsync(Guid? orgId)
        {
            if (!orgId.HasValue) return "Unknown";
            try
            {
                var org = await _userStoreAPI.GetOrgById(orgId.Value);
                return org?.Name ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve organization with ID: {orgId}");
                return "Unknown";
            }
        }

        // Retrieves user full name by ID or returns "Unknown User" if not found
        protected async Task<string> GetUserNameAsync(Guid? userId)
        {
            if (!userId.HasValue) return "Unknown User";
            try
            {
                var user = await _userStoreAPI.GetUserById(userId.Value);
                return user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve user with ID: {userId}");
                return "Unknown User";
            }
        }
    }


    public class EventMessageResult
    {
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
    }
}
