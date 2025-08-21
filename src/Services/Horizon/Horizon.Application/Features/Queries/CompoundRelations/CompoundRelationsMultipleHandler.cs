using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daikon.Shared.VM.Horizon;
using Horizon.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Queries.CompoundRelations
{
    public class CompoundRelationsMultipleHandler : IRequestHandler<CompoundRelationsMultipleQuery, CompoundRelationsMultipleVM>
    {
        private const int BatchSize = 50;
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<CompoundRelationsMultipleHandler> _logger;

        public CompoundRelationsMultipleHandler(
            IGraphQueryRepository graphQueryRepository,
            ILogger<CompoundRelationsMultipleHandler> logger)
        {
            _graphQueryRepository = graphQueryRepository ?? throw new ArgumentNullException(nameof(graphQueryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CompoundRelationsMultipleVM> Handle(CompoundRelationsMultipleQuery request, CancellationToken cancellationToken)
        {
            var result = new Dictionary<Guid, List<CompoundRelationsVM>>();

            if (request.Ids == null || !request.Ids.Any())
            {
                return new CompoundRelationsMultipleVM { Relations = result };
            }

            try
            {
                var idBatches = SplitIntoBatches(request.Ids, BatchSize);

                foreach (var batch in idBatches)
                {
                    var neo4jQuery = @"
                        MATCH (n:Molecule)
                        WHERE n.uniId IN $uniIds
                        MATCH (n)-[r]-(related)
                        OPTIONAL MATCH (related:HitCollection)-[:HIT_COLLECTION_OF]-(s:Screen)
                        RETURN n.uniId AS moleculeId, related, r, s";

                    var parameters = new { uniIds = batch.Select(id => id.ToString()).ToList() };

                    var records = await _graphQueryRepository.RunReadAsync(neo4jQuery, parameters, cancellationToken);

                    foreach (var record in records)
                    {
                        ProcessRecord(record, result);
                    }
                }

                return new CompoundRelationsMultipleVM { Relations = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling compound relations for IDs: {@Ids}", request.Ids);
                throw new ApplicationException("Failed to retrieve compound relations. Please try again later.");
            }
        }

        /// Splits the input list into batches of specified size.
        private static List<List<Guid>> SplitIntoBatches(IEnumerable<Guid> ids, int batchSize)
        {
            return ids.Select((id, index) => new { id, index })
                      .GroupBy(x => x.index / batchSize)
                      .Select(g => g.Select(x => x.id).ToList())
                      .ToList();
        }

        /// Processes a single Neo4j record and adds it to the result dictionary.
        private void ProcessRecord(IRecord record, Dictionary<Guid, List<CompoundRelationsVM>> result)
        {

            var moleculeId = Guid.Parse(record["moleculeId"].As<string>());
            var relatedNode = record["related"].As<INode>();
            var relationship = record["r"].As<IRelationship>();

            var relationVM = new CompoundRelationsVM
            {
                Id = Guid.Parse(relatedNode.Properties["uniId"].As<string>()),
                NodeType = relatedNode.Labels.FirstOrDefault(),
                NodeRelation = relationship.Type,
                NodeName = relatedNode.Properties["name"].As<string>()
            };

            relationVM.NodeProperties["relationship"] = relationship.Type.ToLowerInvariant().Replace("_", " ");

            // Add non-class properties to dynamic dictionary
            foreach (var property in relatedNode.Properties)
            {
                var isStandardProperty = relationVM.GetType().GetProperties()
                    .Any(p => p.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase));

                if (!isStandardProperty && property.Value != null)
                {
                    if (property.Value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                    {
                        relationVM.NodeProperties[property.Key] = strValue;
                    }
                    else if (!(property.Value is string))
                    {
                        relationVM.NodeProperties[property.Key] = property.Value;
                    }
                }
            }

            // Attach screen node properties if available
            if (record.Keys.Contains("s") && record["s"] is INode screenNode)
            {
                SafeAddNodeProperty(screenNode, relationVM, "uniId", "screenId");
                SafeAddNodeProperty(screenNode, relationVM, "screenType");
                SafeAddNodeProperty(screenNode, relationVM, "status", "screenStatus");
                SafeAddNodeProperty(screenNode, relationVM, "primaryOrgId", "orgId");
            }

            if (!result.ContainsKey(moleculeId))
            {
                result[moleculeId] = new List<CompoundRelationsVM>();
            }

            result[moleculeId].Add(relationVM);
        }

        /// Safely adds a property from a Neo4j node to a VM's NodeProperties dictionary.
        private void SafeAddNodeProperty(INode node, CompoundRelationsVM vm, string nodeKey, string vmKey = null)
        {
            if (node.Properties.TryGetValue(nodeKey, out var value))
            {
                vm.NodeProperties[vmKey ?? nodeKey] = value?.ToString();
            }
        }
    }
}
