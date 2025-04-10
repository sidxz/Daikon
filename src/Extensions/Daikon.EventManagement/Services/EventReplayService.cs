/*
 * EventReplayService
 * ------------------
 * A service responsible for replaying domain events from the event store,
 * optionally filtered by event type and time range, and publishing them to Kafka.
 *
 * Key Features:
 * - Supports full replays or filtered subsets of events by date or type.
 * - Can run in "dry run" mode to simulate event publishing without sending to Kafka.
 * - Produces events via a pluggable IEventProducer implementation.
 * - Tracks replay progress and supports cancellation through ReplayStatusTracker.
 * - Logs replay activity in detail, with error handling at both aggregate and event levels.
 *
 * Use Cases:
 * - Backfilling data to downstream services.
 * - Reprocessing events for debugging or analytics.
 * - Simulating replay flows during staging or migration.
 *
 * Notes:
 * - Ensure Kafka settings are correctly configured or passed via topic override.
 * - Events must inherit from BaseEvent to be valid for production.
 */


using System.Text.Json;
using Daikon.EventStore.Event;
using Daikon.EventStore.Models;
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
        private readonly ReplayStatusTracker _replayStatusTracker;
        private readonly ILogger<EventReplayService> _logger;

        public EventReplayService(
            IEventStoreRepository eventStoreRepository,
            IEventProducer eventProducer,
            IKafkaProducerSettings kafkaProducerSettings,
            ReplayStatusTracker replayStatusTracker,
            ILogger<EventReplayService> logger)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
            _kafkaProducerSettings = kafkaProducerSettings;
            _replayStatusTracker = replayStatusTracker;
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
                /* Resolve the Kafka topic using override or fallback to settings */
                var topic = ResolveKafkaTopic(topicOverride);

                /* Log the beginning of the replay operation */
                LogReplayStart(topic, dryRun, eventTypeFilter, fromDate, toDate);

                /* Fetch all aggregate IDs to replay */
                var aggregateIds = (await _eventStoreRepository.GetAllAggregateIds()).ToList();

                /* Initialize the tracker to monitor progress */
                _replayStatusTracker.Start(aggregateIds.Count, dryRun, eventTypeFilter, fromDate, toDate);

                /* Process each aggregate one by one */
                foreach (var aggregateId in aggregateIds)
                {
                    /* Check if the replay was cancelled mid-run */
                    if (_replayStatusTracker.CancelToken?.IsCancellationRequested == true)
                    {
                        _logger.LogWarning("🔁 Replay cancelled by user.");
                        break;
                    }

                    /* Process and replay events for a single aggregate */
                    await ProcessAggregateEventsAsync(aggregateId, topic, dryRun, eventTypeFilter, fromDate, toDate);
                }

                /* Finalize tracker and log completion */
                _replayStatusTracker.Finish();
                _logger.LogInformation("✅ Replay completed.");
            }
            catch (Exception ex)
            {
                /* Catch any fatal exceptions, finalize tracker, and rethrow */
                _logger.LogCritical(ex, "💣 Fatal error during event replay.");
                _replayStatusTracker.Finish();
                throw;
            }
        }

        private string ResolveKafkaTopic(string? topicOverride)
        {
            /* Determine which topic to use: override > settings > default */
            var resolvedTopic = topicOverride ?? _kafkaProducerSettings.Topic ?? "default_topic";

            if (string.IsNullOrWhiteSpace(resolvedTopic))
            {
                throw new ArgumentException("Kafka topic must be specified.", nameof(resolvedTopic));
            }

            /* Log the resolved Kafka producer settings */
            _logger.LogInformation("Kafka settings: BootstrapServers={BootstrapServers}, Topic={Topic}",
                _kafkaProducerSettings.BootstrapServers, resolvedTopic);

            return resolvedTopic;
        }

        private void LogReplayStart(
            string topic, bool dryRun, string? eventTypeFilter, DateTime? fromDate, DateTime? toDate)
        {
            /* Log initial replay configuration */
            _logger.LogInformation("Replay starting: dryRun={DryRun}, topic={Topic}, from={From}, to={To}, filter={EventTypeFilter}",
                dryRun, topic, fromDate, toDate, eventTypeFilter);
        }

        private async Task ProcessAggregateEventsAsync(
            Guid aggregateId,
            string topic,
            bool dryRun,
            string? eventTypeFilter,
            DateTime? fromDate,
            DateTime? toDate)
        {
            try
            {
                /* Fetch all events associated with the given aggregate ID */
                var events = await _eventStoreRepository.FindByAggregateId(aggregateId);

                /* Apply filters based on type and time window, then sort by version */
                var filteredEvents = events
                    .Where(e =>
                        (string.IsNullOrEmpty(eventTypeFilter) || e.EventType == eventTypeFilter) &&
                        (!fromDate.HasValue || e.TimeStamp >= fromDate.Value) &&
                        (!toDate.HasValue || e.TimeStamp <= toDate.Value))
                    .OrderBy(e => e.Version)
                    .ToList();

                /* Track progress for this aggregate */
                _replayStatusTracker.UpdateCurrent(aggregateId, filteredEvents.Count);

                /* Replay each filtered event */
                foreach (var eventModel in filteredEvents)
                {
                    await HandleEventReplayAsync(eventModel, aggregateId, topic, dryRun);
                }

                /* Mark this aggregate as fully processed */
                _replayStatusTracker.MarkAggregateComplete(filteredEvents.Count);
            }
            catch (Exception ex)
            {
                /* Log and skip on aggregate-level failure */
                _logger.LogError(ex, "💥 Error processing aggregate {AggregateId}. Skipping.", aggregateId);
            }
        }

        private async Task HandleEventReplayAsync(
            EventModel eventModel,
            Guid aggregateId,
            string topic,
            bool dryRun)
        {
            try
            {
                /* Verify the event data is of the expected BaseEvent type */
                if (eventModel.EventData is not BaseEvent baseEvent)
                {
                    _logger.LogWarning("⚠️ Event data is not a BaseEvent: {EventType}. Actual Type: {ActualType}",
                        eventModel.EventType,
                        eventModel.EventData?.GetType().FullName ?? "null");
                    return;
                }

                var eventType = eventModel.EventType;

                /* Log the event type and aggregate before producing */
                _logger.LogInformation("📦 Preparing to replay {EventType} for Aggregate {AggregateId}", eventType, aggregateId);

                if (dryRun)
                {
                    /* Log dry-run mode — no actual Kafka production */
                    _logger.LogInformation("🧪 [DryRun] Would produce {EventType} for Aggregate {AggregateId}", eventType, aggregateId);
                }
                else
                {
                    /* Serialize event data for logging */
                    var serializedPayload = JsonSerializer.Serialize(baseEvent, baseEvent.GetType());

                    /* Log and produce event */
                    _logger.LogInformation("✅ Producing {EventType} for Aggregate {AggregateId}", eventType, aggregateId);
                    _logger.LogDebug("Payload: {Payload}", serializedPayload);

                    await _eventProducer.ProduceAsync(topic, baseEvent);
                }
            }
            catch (Exception ex)
            {
                /* Log and skip failed event */
                _logger.LogError(ex, "💥 Error processing event {EventType} for Aggregate {AggregateId}. Skipping event.",
                    eventModel.EventType, aggregateId);
            }
        }
    }
}
