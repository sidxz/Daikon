using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistance;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Calculation
{
    public class FindRoot
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<FindRoot> _logger;

        public FindRoot(IGraphQueryRepository graphQueryRepository, ILogger<FindRoot> logger)
        {
            _graphQueryRepository = graphQueryRepository;
            _logger = logger;
        }

        public async Task<string> ByUniId(string uniId)
        {

            // Case when root is gene
            _logger.LogInformation("=======================================================ROOT START=======================================================");
            var runQuery = @"MATCH (i {uniId: $uniId}) -[*]-> (g:Gene) RETURN g";
            var parameters = new Dictionary<string, object> { { "uniId", uniId } };


            var cursor = await _graphQueryRepository.RunAsync(runQuery, parameters);
            var records = await cursor.ToListAsync();

            var recordsJson = System.Text.Json.JsonSerializer.Serialize(records);

            _logger.LogInformation($"Setting Root Node....");
            _logger.LogInformation($"Root Node: {recordsJson}");

            _logger.LogInformation("=======================================================ROOT END=======================================================");

            
            return "root";
        }
    }
}