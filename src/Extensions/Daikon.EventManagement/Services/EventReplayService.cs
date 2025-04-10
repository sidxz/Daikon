
using System.Text.Json;
using Daikon.EventStore.Event;
using Daikon.EventStore.Producers;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Settings;

namespace Daikon.EventManagement.Services
{
    public class EventReplayService
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;
        private readonly IKafkaProducerSettings _kafkaProducerSettings;
        private readonly ReplayStatusTracker _tracker;
        private readonly ILogger<EventReplayService> _logger;

        public EventReplayService(
            IEventStoreRepository eventStoreRepository,
            IEventProducer eventProducer,
            IKafkaProducerSettings kafkaProducerSettings,
            ReplayStatusTracker tracker,
            ILogger<EventReplayService> logger)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
            _kafkaProducerSettings = kafkaProducerSettings;
            _tracker = tracker;
            _logger = logger;
        }

        public async Task ReplayAllEventsAsync(
    string? topicOverride = null,
    bool dryRun = false,
    string? eventTypeFilter = null,
    DateTime? fromDate = null,
    DateTime? toDate = null)
        {
            try
            {
                _logger.LogInformation("Replay starting: dryRun={dryRun}, topic={topic}, from={fromDate}, to={toDate}, filter={eventTypeFilter}",
                    dryRun, topicOverride, fromDate, toDate, eventTypeFilter);

                var topic = topicOverride ?? _kafkaProducerSettings.Topic ?? "default_topic";
                if (string.IsNullOrWhiteSpace(topic))
                    throw new ArgumentException("Kafka topic must be specified.", nameof(topic));

                _logger.LogInformation("Kafka settings: BootstrapServers={Bootstrap}, Topic={Topic}", _kafkaProducerSettings.BootstrapServers, topic);

                var aggregateIds = (await _eventStoreRepository.GetAllAggregateIds()).ToList();

                _tracker.Start(aggregateIds.Count, dryRun, eventTypeFilter, fromDate, toDate);

                foreach (var aggregateId in aggregateIds)
                {
                    if (_tracker.CancelToken?.IsCancellationRequested == true)
                    {
                        _logger.LogWarning("🔁 Replay cancelled by user.");
                        break;
                    }

                    try
                    {
                        var events = await _eventStoreRepository.FindByAggregateId(aggregateId);
                        var filteredEvents = events
                            .Where(e =>
                                (string.IsNullOrEmpty(eventTypeFilter) || e.EventType == eventTypeFilter) &&
                                (!fromDate.HasValue || e.TimeStamp >= fromDate.Value) &&
                                (!toDate.HasValue || e.TimeStamp <= toDate.Value))
                            .OrderBy(e => e.Version)
                            .ToList();

                        _tracker.UpdateCurrent(aggregateId, filteredEvents.Count);

                        foreach (var eventModel in filteredEvents)
                        {
                            try
                            {
                                _logger.LogInformation("🙅‍♀️ Dealing {EventType} for Aggregate {Id}", eventModel.GetType().Name, aggregateId);

                                if (eventModel.EventData is BaseEvent baseEvent)
                                {
                                    if (dryRun)
                                    {
                                        _logger.LogInformation("🧪 [DryRun] Would produce {EventType} for Aggregate {Id}", eventModel.EventType, aggregateId);
                                    }
                                    else
                                    {
                                        _logger.LogInformation("✅ Producing {EventType} for Aggregate {Id}", eventModel.EventType, aggregateId);
                                        _logger.LogInformation("Payload: {Payload}", JsonSerializer.Serialize(baseEvent, baseEvent.GetType()));

                                        await _eventProducer.ProduceAsync(topic, baseEvent);
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("⚠️ Event data is not a BaseEvent: {EventType}. Actual Type: {ActualType}",
                                        eventModel.EventType,
                                        eventModel.EventData?.GetType().FullName ?? "null");
                                }
                            }
                            catch (Exception eventEx)
                            {
                                _logger.LogError(eventEx,
                                    "💥 Error processing event {EventType} for Aggregate {Id}. Skipping this event.",
                                    eventModel.EventType, aggregateId);
                            }
                        }

                        _tracker.MarkAggregateComplete(filteredEvents.Count);
                    }
                    catch (Exception aggEx)
                    {
                        _logger.LogError(aggEx, "💥 Error processing aggregate {AggregateId}. Skipping this aggregate.", aggregateId);
                    }
                }

                _tracker.Finish();
                _logger.LogInformation("✅ Replay completed.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "💣 Fatal error during ReplayAllEventsAsync.");
                _tracker.Finish();
                throw;
            }
        }

    }

}