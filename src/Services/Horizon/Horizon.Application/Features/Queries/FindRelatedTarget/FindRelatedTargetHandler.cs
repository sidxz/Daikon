using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Queries.FindRelatedTarget
{
    public class FindRelatedTargetHandler : IRequestHandler<FindRelatedTargetQuery, RelatedTargetVM>
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<FindRelatedTargetHandler> _logger;

        public FindRelatedTargetHandler(IGraphQueryRepository graphQueryRepository, ILogger<FindRelatedTargetHandler> logger)
        {
            _graphQueryRepository = graphQueryRepository;
            _logger = logger;
        }

        public async Task<RelatedTargetVM> Handle(FindRelatedTargetQuery request, CancellationToken cancellationToken)
        {
            // Query to fetch the root node and its connected nodes.
            try
            {
                var query = @"MATCH (t:Target)<-[*]-(x {uniId: $uniId})
                                RETURN DISTINCT t";
                var parameters = new Dictionary<string, object> { { "uniId", request.Id.ToString() } };

                var cursor = await _graphQueryRepository.RunAsync(query, parameters);

                var record = await cursor.SingleAsync();

                var node = record["t"].As<INode>();

                return new RelatedTargetVM
                {
                    Id = request.Id,
                    TargetId = Guid.Parse(node.Properties["uniId"].As<string>()),
                    Name = node.Properties["name"].As<string>(),
                    TargetType = node.Properties["targetType"].As<string>()
                };
            }
            catch (InvalidOperationException)
            {
                _logger.LogError($"No related target found for ID: {request.Id}");
                return new RelatedTargetVM
                {
                    Id = request.Id,
                    TargetId = Guid.Empty,
                    Name = null,
                    TargetType = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the query.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }
    }
}