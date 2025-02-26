using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Queries.CompoundRelations
{
    public class CompoundRelationsMultipleHandler : IRequestHandler<CompoundRelationsMultipleQuery, CompoundRelationsMultipleVM>
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<CompoundRelationsMultipleHandler> _logger;

        public CompoundRelationsMultipleHandler(IGraphQueryRepository graphQueryRepository, ILogger<CompoundRelationsMultipleHandler> logger)
        {
            _graphQueryRepository = graphQueryRepository ?? throw new ArgumentNullException(nameof(graphQueryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CompoundRelationsMultipleVM> Handle(CompoundRelationsMultipleQuery request, CancellationToken cancellationToken)
        {
            if (request.Ids == null || !request.Ids.Any())
            {
                return new CompoundRelationsMultipleVM { Relations = new Dictionary<Guid, List<CompoundRelationsVM>>() };
            }

            try
            {
                const int batchSize = 50; // Split into batches of 50 IDs
                var results = new Dictionary<Guid, List<CompoundRelationsVM>>();

                var idBatches = request.Ids.Select((id, index) => new { id, index })
                                           .GroupBy(x => x.index / batchSize)
                                           .Select(g => g.Select(x => x.id).ToList())
                                           .ToList();

                foreach (var batch in idBatches)
                {
                    var query = @"
                MATCH (n:Molecule)-[r]-(related)
                WHERE n.uniId IN $uniIds
                RETURN n.uniId AS moleculeId, n, r, related";

                    var parameters = new Dictionary<string, object> { { "uniIds", batch.Select(id => id.ToString()).ToList() } };
                    var cursor = await _graphQueryRepository.RunAsync(query, parameters);

                    while (await cursor.FetchAsync())
                    {
                        var moleculeId = Guid.Parse(cursor.Current["moleculeId"].As<string>());
                        var node = cursor.Current["related"].As<INode>();
                        var relation = cursor.Current["r"].As<IRelationship>();

                        var vm = new CompoundRelationsVM
                        {
                            Id = Guid.Parse(node.Properties["uniId"].As<string>()),
                            NodeType = node.Labels.FirstOrDefault(),
                            NodeRelation = relation.Type,
                            NodeName = node.Properties["name"].As<string>(),
                        };

                        vm.NodeProperties["relationship"] = relation.Type.ToLower().Replace("_", " ");

                        foreach (var property in node.Properties)
                        {
                            if (!vm.GetType().GetProperties().Any(p => p.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase)))
                            {
                                if (property.Value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                                {
                                    vm.NodeProperties[property.Key] = strValue;
                                }
                                else if (!(property.Value is string))
                                {
                                    vm.NodeProperties[property.Key] = property.Value;
                                }
                            }
                        }

                        if (!results.ContainsKey(moleculeId))
                        {
                            results[moleculeId] = new List<CompoundRelationsVM>();
                        }
                        results[moleculeId].Add(vm);
                    }
                }

                return new CompoundRelationsMultipleVM { Relations = results };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching compound relations.");
                throw;
            }
        }
    }
}
