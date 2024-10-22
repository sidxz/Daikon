using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using Daikon.Events.HitAssessment;
using EventHistory.Application.Contracts.Persistence;
using EventHistory.Application.Features.Processors;
using EventHistory.Application.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Queries.GetEventHistory
{
    public class GetEventHistoryHandler : IRequestHandler<GetEventHistoryQuery, List<EventHistoryVM>>
    {
        private readonly IEventStoreRepositoryExtension _eventStoreRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEventHistoryHandler> _logger;
        private EventToMessage _eventToMessage;

        public GetEventHistoryHandler(
            IEventStoreRepositoryExtension eventStoreRepository,
            IMapper mapper,
            ILogger<GetEventHistoryHandler> logger, EventToMessage eventToMessage)
        {
            _eventStoreRepository = eventStoreRepository
                ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            _eventToMessage = eventToMessage ?? throw new ArgumentNullException(nameof(eventToMessage));
        }

        public async Task<List<EventHistoryVM>> Handle(GetEventHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch event history from repository with validated parameters
                var eventLogs = await _eventStoreRepository.GetHistoryAsync(
                    request.AggregateIds,
                    request.AggregateTypes,
                    request.EventTypes,
                    request.StartDate,
                    request.EndDate,
                    request.Limit
                );

                if (eventLogs == null || !eventLogs.Any())
                {
                    _logger.LogInformation("No event logs found for the provided filters.");
                    return new List<EventHistoryVM>();
                }

                // Initialize the ViewModel list
                var eventHistoryViewModels = new List<EventHistoryVM>();
                foreach (var eventLog in eventLogs)
                {
                    var eventData = eventLog.EventData;
                    var eventHistoryVM = await CreateEventHistoryVM(eventLog, eventData);

                    if (eventHistoryVM != null)
                    {
                        eventHistoryViewModels.Add(eventHistoryVM);
                    }
                    else
                    {
                        _logger.LogWarning($"Unsupported event type: {eventData?.GetType().Name}");
                    }
                }

                return eventHistoryViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching event history.");
                throw;  // Consider wrapping in a custom exception for better context
            }
        }

        /// <summary>
        /// Factory method to create EventHistoryVM based on event type.
        /// </summary>
        /// <param name="eventLog">Event log data</param>
        /// <param name="eventData">Event data object</param>
        /// <returns>EventHistoryVM or null if event type is unsupported</returns>
        private async Task<EventHistoryVM> CreateEventHistoryVM(EventModel eventLog, BaseEvent eventData)
        {
            if (eventData == null)
            {
                _logger.LogWarning("Event data is null, skipping log entry.");
                return null;
            }

            // Process event data to get a message and link
            var eventMessageResult = await _eventToMessage.Process(eventData);

            return new EventHistoryVM
            {
                Id = eventLog.AggregateIdentifier,
                TimeStamp = eventLog.TimeStamp,
                AggregateType = eventLog.AggregateType,
                Version = eventLog.Version,
                EventType = eventLog.EventType,
                EventMessage = eventMessageResult?.Message,  // Handle potential nulls gracefully
                UserId = eventLog.UserId,
                Link = eventMessageResult?.Link
            };
        }
    }
}
