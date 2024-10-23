
using AutoMapper;
using CQRS.Core.Event;
using Daikon.Events.HitAssessment;
using Daikon.Events.Screens;
using Daikon.Shared.APIClients.HitAssessment;
using Daikon.Shared.APIClients.UserStore;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private readonly IUserStoreAPI _userStoreAPI;
        private readonly IHitAssessmentAPI _hitAssessmentAPI;
        private readonly IMapper _mapper;
        private readonly ILogger<EventMessageProcessor> _logger;

        public EventMessageProcessor(
            IUserStoreAPI userStoreAPI,
            IHitAssessmentAPI hitAssessmentAPI,
            IMapper mapper,
            ILogger<EventMessageProcessor> logger)
        {
            _userStoreAPI = userStoreAPI ?? throw new ArgumentNullException(nameof(userStoreAPI));
            _hitAssessmentAPI = hitAssessmentAPI ?? throw new ArgumentNullException(nameof(hitAssessmentAPI));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<EventMessageResult> Process(BaseEvent eventData)
        {
            try
            {
                return eventData switch
                {
                    /* Screen */
                    ScreenCreatedEvent screenCreatedEvent => await HandleScreenCreatedEventAsync(screenCreatedEvent),
                    /* HIt Assessment */
                    HaCreatedEvent haCreatedEvent => await HandleHaCreatedEvent(haCreatedEvent),
                    HaUpdatedEvent haUpdatedEvent => await HandleHaUpdatedEvent(haUpdatedEvent),
                    HaDeletedEvent haDeletedEvent => HandleHaDeletedEvent(haDeletedEvent),
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
    }

    public class EventMessageResult
    {
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
