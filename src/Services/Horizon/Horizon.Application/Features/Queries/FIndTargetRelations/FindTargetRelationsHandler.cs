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
    public class FindTargetRelationsHandler : IRequestHandler<FindTargetRelationsCommand, TargetRelationsVM>
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<FindTargetRelationsHandler> _logger;

        public FindTargetRelationsHandler(IGraphQueryRepository graphQueryRepository, ILogger<FindTargetRelationsHandler> logger)
        {
            _graphQueryRepository = graphQueryRepository;
            _logger = logger;
        }

        public async Task<TargetRelationsVM> Handle(FindTargetRelationsCommand request, CancellationToken cancellationToken)
        {

            var query = @"MATCH (t:Target {uniId: $uniId}) <-[rel*]- (x) RETURN x";
            var parameters = new Dictionary<string, object> { { "uniId", request.UniId } };

            var cursor = await _graphQueryRepository.RunAsync(query, parameters);
            var records = await cursor.ToListAsync(cancellationToken);

            var highestRelationshipScore = 1;

            foreach (var record in records)
            {
                var node = record["x"].As<INode>();
                var nodeType = node.Labels.First();
                var nodeProperties = node.Properties;

                if (nodeType == "Target")
                {
                    highestRelationshipScore = highestRelationshipScore > 1 ? highestRelationshipScore : 1;
                }
                else if (nodeType == "Screen")
                {
                    highestRelationshipScore = highestRelationshipScore > 2 ? highestRelationshipScore : 2;
                }
                else if (nodeType == "HitCollection")
                {
                    highestRelationshipScore = highestRelationshipScore > 3 ? highestRelationshipScore : 3;
                }
                else if (nodeType == "HitAssessment")
                {
                    highestRelationshipScore = highestRelationshipScore > 4 ? highestRelationshipScore : 4;
                }
                else if (nodeType == "Project")
                {
                    highestRelationshipScore = highestRelationshipScore > 5 ? highestRelationshipScore : 5;

                    if (nodeProperties["stage"].ToString() == "IND" || nodeProperties["stage"].ToString() == "P1")
                    {
                        highestRelationshipScore = highestRelationshipScore > 6 ? highestRelationshipScore : 6;
                    }
                }
            }

            var highestRelationship = "Target";

            switch (highestRelationshipScore)
            {
                case 2:
                    highestRelationship = "Screen";
                    break;
                case 3:
                    highestRelationship = "Screen";
                    break;
                case 4:
                    highestRelationship = "HitAssessment";
                    break;
                case 5:
                    highestRelationship = "Portfolio";
                    break;
                case 6:
                    highestRelationship = "PostPortfolio";
                    break;
            }

            var targetRelations = new TargetRelationsVM
            {
                TargetId = request.UniId,
                HighestRelationship = highestRelationship
            };

            return targetRelations;
        }
    }
}