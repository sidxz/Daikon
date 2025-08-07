using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventHistory.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace EventHistory.Application.Features.Queries.GetAggregateHistory
{
    public class GetAggregateHistoryHandler : IRequestHandler<GetAggregateHistoryQuery, string>
    {
        private readonly IEventStoreRepositoryExtension _eventStoreRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAggregateHistoryHandler> _logger;

        public GetAggregateHistoryHandler(
            IEventStoreRepositoryExtension eventStoreRepository,
            IMapper mapper,
            ILogger<GetAggregateHistoryHandler> logger)
        {
            _eventStoreRepository = eventStoreRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> Handle(GetAggregateHistoryQuery request, CancellationToken cancellationToken)
        {
            var aggregateIds = new List<Guid> { request.AggregateId };
            var eventLogs = await _eventStoreRepository.GetHistoryAsync(
                    aggregateIds,
                    request.AggregateTypes,
                    request.EventTypes,
                    request.StartDate,
                    request.EndDate,
                    request.Limit
                );


            
            return eventLogs.ToJson(new MongoDB.Bson.IO.JsonWriterSettings
            {
                Indent = true,
                OutputMode = MongoDB.Bson.IO.JsonOutputMode.CanonicalExtendedJson
            });
        }
    }
}