using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Queries.FIndTargetRelations
{
    public class ListAllTargetRelationsHandler : IRequestHandler<ListAllTargetRelationsQuery, List<TargetRelationsVM>>
    {

        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<ListAllTargetRelationsHandler> _logger;
        private readonly IMediator _mediator;

        public ListAllTargetRelationsHandler(IGraphQueryRepository graphQueryRepository, ILogger<ListAllTargetRelationsHandler> logger, IMediator mediator)
        {
            _graphQueryRepository = graphQueryRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<List<TargetRelationsVM>> Handle(ListAllTargetRelationsQuery request, CancellationToken cancellationToken)
        {
            // Graph query to get all target relations
            var query = @"MATCH (t:Target) RETURN t";
            var parameters = new { };
            var targets = await _graphQueryRepository.RunReadAsync(query, parameters, cancellationToken);

            var targetRelations = new List<TargetRelationsVM>();

            foreach (var target in targets)
            {
                var targetId = target["t"].As<INode>().Properties["uniId"].ToString();
                var targetName = target["t"].As<INode>().Properties["name"].ToString();
                var highestRelationship = await _mediator.Send(new FindTargetRelationsQuery { UniId = targetId }, cancellationToken);

                var targetRelation = new TargetRelationsVM
                {
                    TargetId = targetId,
                    TargetName = targetName,
                    HighestRelationship = highestRelationship.HighestRelationship
                };

                targetRelations.Add(targetRelation);
            }

            return targetRelations;

        }
    }
}