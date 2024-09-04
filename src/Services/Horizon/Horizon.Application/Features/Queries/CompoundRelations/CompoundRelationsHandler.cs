using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Queries.CompoundRelations
{
    /// <summary>
    /// Handler for fetching compound relations and mapping them to the view model.
    /// </summary>
    public class CompoundRelationsHandler : IRequestHandler<CompoundRelationsQuery, List<CompoundRelationsVM>>
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<CompoundRelationsHandler> _logger;

        public CompoundRelationsHandler(IGraphQueryRepository graphQueryRepository, ILogger<CompoundRelationsHandler> logger)
        {
            _graphQueryRepository = graphQueryRepository ?? throw new ArgumentNullException(nameof(graphQueryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<CompoundRelationsVM>> Handle(CompoundRelationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Query to fetch the molecule and its related nodes
                var query = @"MATCH (n:Molecule {uniId: $uniId})-[r]-(related)
                              RETURN n, r, related";
                var parameters = new Dictionary<string, object> { { "uniId", request.Id.ToString() } };

                var cursor = await _graphQueryRepository.RunAsync(query, parameters);

                var results = new List<CompoundRelationsVM>();

                while (await cursor.FetchAsync())
                {
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



                    // Add any additional properties from the node
                    foreach (var property in node.Properties)
                    {
                        if (!vm.GetType().GetProperties().Any(p => p.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase)))
                        {
                            // Check if the property value is not null or whitespace before adding it
                            if (property.Value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                            {
                                vm.NodeProperties[property.Key] = strValue;
                            }
                            else if (!(property.Value is string)) // Add non-string properties as they are
                            {
                                vm.NodeProperties[property.Key] = property.Value;
                            }
                        }
                    }

                    // If the node type is 'HitCollection', query for related screen information
                    if (vm.NodeType == "HitCollection")
                    {
                        var screenQuery = @"MATCH (hc:HitCollection {uniId: $hitCollectionId}) -[r:HIT_COLLECTION_OF]-(s:Screen) RETURN s";
                        var screenParameters = new Dictionary<string, object> { { "hitCollectionId", node.Properties["uniId"].As<string>() } };

                        var screenCursor = await _graphQueryRepository.RunAsync(screenQuery, screenParameters);
                        if (await screenCursor.FetchAsync())
                        {
                            var screenNode = screenCursor.Current["s"].As<INode>();

                            // Append Screen properties to the VM
                            vm.NodeProperties["screenId"] = screenNode.Properties["uniId"].As<string>();
                            vm.NodeProperties["screenType"] = screenNode.Properties["screenType"].As<string>();
                            vm.NodeProperties["screenStatus"] = screenNode.Properties["status"].As<string>();
                            vm.NodeProperties["orgId"] = screenNode.Properties["primaryOrgId"].As<string>();
                        }
                    }

                    results.Add(vm);
                }

                return results;
            }
            catch (InvalidOperationException)
            {
                _logger.LogError($"No related nodes found for Molecule ID: {request.Id}");
                return new List<CompoundRelationsVM>(); // Return an empty list if no related nodes are found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the query.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }
    }
}
