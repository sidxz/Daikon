using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Daikon.Events.HitAssessment;
using Daikon.EventStore.Settings;
using EventHistory.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace EventHistory.Infrastructure.Query.Repositories
{
    public class EventStoreRepositoryExtension : IEventStoreRepositoryExtension
    {

        private readonly IMongoCollection<EventModel> _eventStoreCollection;
        private readonly ILogger<EventStoreRepositoryExtension> _logger;

        public EventStoreRepositoryExtension(IEventDatabaseSettings settings, ILogger<EventStoreRepositoryExtension> logger)
        {
            // Initialize MongoDB client and collection
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _eventStoreCollection = database.GetCollection<EventModel>(settings.CollectionName);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /*
        GetHistoryAsync:

        Retrieves event history based on optional filters such as aggregateId, aggregateType, eventType, 
        with optional date range and result limit.
       */
        public async Task<List<EventModel>> GetHistoryAsync(
            List<Guid> aggregateIds = null,
            List<string> aggregateTypes = null,
            List<string> eventTypes = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int limit = 100)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ArgumentException("Start date must be earlier than or equal to the end date.");

            var filterBuilder = Builders<EventModel>.Filter;
            var filters = new List<FilterDefinition<EventModel>>();

            // Apply date range filters
            if (startDate.HasValue)
                filters.Add(filterBuilder.Gte(x => x.TimeStamp, startDate.Value));

            if (endDate.HasValue)
                filters.Add(filterBuilder.Lte(x => x.TimeStamp, endDate.Value));

            // Apply aggregateId filter for list
            if (aggregateIds != null && aggregateIds.Any())
                filters.Add(filterBuilder.In(x => x.AggregateIdentifier, aggregateIds));

            // Apply aggregateType filter for list
            if (aggregateTypes != null && aggregateTypes.Any())
                filters.Add(filterBuilder.In(x => x.AggregateType, aggregateTypes));

            // Apply eventType filter for list
            if (eventTypes != null && eventTypes.Any())
                filters.Add(filterBuilder.In(x => x.EventType, eventTypes));

            var combinedFilter = filters.Any()
                ? filterBuilder.And(filters)
                : FilterDefinition<EventModel>.Empty;

            try
            {
                var events = await _eventStoreCollection
                .Find(combinedFilter)
                .SortByDescending(x => x.TimeStamp)
                .Limit(limit)
                .ToListAsync()
                .ConfigureAwait(false);
                return events.Where(e => e.EventData != null).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event history.");
                throw new ApplicationException("An error occurred while fetching event history.", ex);
            }
        }
    }
}