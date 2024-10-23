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
using Microsoft.Extensions.Caching.Memory;

namespace EventHistory.Application.Features.Queries.GetEventHistory
{
    public class GetEventHistoryHandler : IRequestHandler<GetEventHistoryQuery, List<EventHistoryVM>>
    {
        private readonly IEventStoreRepositoryExtension _eventStoreRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEventHistoryHandler> _logger;
        private readonly EventMessageProcessor _eventMessageProcessor;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);  // Cache duration set to 30 minutes

        public GetEventHistoryHandler(
            IEventStoreRepositoryExtension eventStoreRepository,
            IMapper mapper,
            ILogger<GetEventHistoryHandler> logger,
            EventMessageProcessor eventMessageProcessor,
            IMemoryCache memoryCache)
        {
            _eventStoreRepository = eventStoreRepository
                ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            _eventMessageProcessor = eventMessageProcessor
                ?? throw new ArgumentNullException(nameof(eventMessageProcessor));
            _memoryCache = memoryCache
                ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<List<EventHistoryVM>> Handle(GetEventHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Define a cache key based on the query parameters
                var cacheKey = GetCacheKey(request);

                // Try to get data from the cache
                if (_memoryCache.TryGetValue(cacheKey, out List<EventHistoryVM> cachedEventHistory))
                {
                    _logger.LogInformation("Returning cached event history.");
                    return cachedEventHistory;
                }

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
                }

                // Store the result in cache for 30 minutes
                _memoryCache.Set(cacheKey, eventHistoryViewModels, _cacheDuration);

                return eventHistoryViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching event history.");
                throw;  // Consider wrapping in a custom exception for better context
            }
        }

        private string GetCacheKey(GetEventHistoryQuery request)
        {
            var aggregateIds = request.AggregateIds != null && request.AggregateIds.Any()
                ? string.Join("_", request.AggregateIds)
                : "no-aggregate-ids";

            var aggregateTypes = request.AggregateTypes != null && request.AggregateTypes.Any()
                ? string.Join("_", request.AggregateTypes)
                : "no-aggregate-types";

            var eventTypes = request.EventTypes != null && request.EventTypes.Any()
                ? string.Join("_", request.EventTypes)
                : "no-event-types";

            var startDate = request.StartDate?.ToString("o") ?? "no-start-date";  // 'o' for round-trip date/time pattern
            var endDate = request.EndDate?.ToString("o") ?? "no-end-date";

            return $"{aggregateIds}_{aggregateTypes}_{eventTypes}_{startDate}_{endDate}_{request.Limit}";
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
            var eventMessageResult = await _eventMessageProcessor.Process(eventData);

            if (eventMessageResult.Message == "Unsupported event")
            {
                return null;
            }

            return new EventHistoryVM
            {
                Id = eventLog.Id,
                AggregateIdentifier = eventLog.AggregateIdentifier,
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
