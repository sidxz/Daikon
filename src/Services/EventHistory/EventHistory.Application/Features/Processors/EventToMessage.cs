using System;
using AutoMapper;
using CQRS.Core.Event;
using Daikon.Events.HitAssessment;
using Daikon.Shared.APIClients.UserStore;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public class EventToMessage
    {
        private readonly IUserStoreAPI _userStoreAPI;
        private readonly IMapper _mapper;
        private readonly ILogger<EventToMessage> _logger;

        public EventToMessage(IUserStoreAPI userStoreAPI, IMapper mapper, ILogger<EventToMessage> logger)
        {
            _userStoreAPI = userStoreAPI ?? throw new ArgumentNullException(nameof(userStoreAPI));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<EventMessageResult> Process(BaseEvent eventData)
        {
            if (eventData is HaCreatedEvent haCreatedEvent)
            {
                var primaryOrg = await _userStoreAPI.GetOrgById(haCreatedEvent.PrimaryOrgId);
                var primaryOrgName = primaryOrg?.Name ?? "Unknown";

                return new EventMessageResult
                {
                    Message = $"HA {haCreatedEvent.Name} created by {primaryOrgName}",
                    Link = $"/ha/{haCreatedEvent.Id}"
                };
            }
            else if (eventData is HaUpdatedEvent haUpdatedEvent)
            {
                return new EventMessageResult
                {
                    Message = $"HA {haUpdatedEvent.Id} updated",
                    Link = $"/ha/{haUpdatedEvent.Id}/updated"
                };
            }
            else if (eventData is HaDeletedEvent haDeletedEvent)
            {
                return new EventMessageResult
                {
                    Message = $"HA {haDeletedEvent.Id} deleted",
                    Link = $"/ha/{haDeletedEvent.Id}/deleted"
                };
            }

            // Return a default response if the event is unsupported
            return new EventMessageResult
            {
                Message = "Unsupported event",
                Link = $"/ha/unsupported"
            };
        }
    }

    /// <summary>
    /// Class to encapsulate both message and link returned from event processing.
    /// </summary>
    public class EventMessageResult
    {
        public string Message { get; set; } = "";
        public string Link { get; set; } = "";
    }
}
