
using Horizon.Application.Contracts.Persistance;
using Horizon.Application.Features.Calculation;
using Horizon.Application.VMs.D3;
using MediatR;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Queries.GenerateHorizon
{
    public class GenerateHorizonHandler : IRequestHandler<GenerateHorizonQuery, D3Node>
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<GenerateHorizonHandler> _logger;
        private readonly RootFinder _rootFinder;

        public GenerateHorizonHandler(IGraphQueryRepository graphQueryRepository, ILogger<GenerateHorizonHandler> logger, RootFinder rootFinder)
        {
            _graphQueryRepository = graphQueryRepository ?? throw new ArgumentNullException(nameof(graphQueryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rootFinder = rootFinder ?? throw new ArgumentNullException(nameof(rootFinder));
        }

        public async Task<D3Node> Handle(GenerateHorizonQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting horizon generation...");

            // Find the root node using the unique ID from the request.
            var rootId = await _rootFinder.FindByUniqueIdAsync(request.Id.ToString());
            if (rootId == null)
            {
                _logger.LogError($"Root node not found for ID: {request.Id}");
                return CreateErrorNode();
            }

            // Query to fetch the root node and its connected nodes.
            var query = @"MATCH (root {uniId : $uniId}) <-[rel*]-(node)
                          WHERE node:Gene OR node:Target OR node:Screen OR node:HitCollection OR node:HitAssessment OR node:Project
                          RETURN root, rel, node";
            var parameters = new Dictionary<string, object> { { "uniId", rootId } };
            var cursor = await _graphQueryRepository.RunAsync(query, parameters);
            var records = await cursor.ToListAsync(cancellationToken);

            // Process the records to construct the D3Node tree.
            var rootNode = ProcessRecords(records);
            if (rootNode == null)
            {
                _logger.LogError($"Failed to construct the D3Node tree for root ID: {rootId}");
                return CreateErrorNode();
            }

            _logger.LogInformation("Horizon generation completed.");
            return rootNode;
        }

        /// <summary>
        /// Represents a node in the D3 visualization.
        /// </summary>
        private D3Node ProcessRecords(IList<IRecord> records)
        {
            if (records.Count == 0)
            {
                return null;
            }

            // Initialize root and branch nodes.
            var rootNode = new D3Node();
            var branchNodes = new Dictionary<string, D3Node>();

            // Set the root node properties.
            SetRootNodeProperties(rootNode, records[0]["root"].As<INode>());

            // Iterate through records to set branch nodes and relationships.
            foreach (var record in records)
            {
                var node = record["node"].As<INode>();
                var branchNode = CreateD3NodeFromINode(node);
                branchNodes[branchNode.NeoId] = branchNode;
            }
            foreach (var record in records)
            {
                var relationships = record["rel"].As<List<IRelationship>>();
                SetRelationships(rootNode, branchNodes, relationships);
            }

            return rootNode;
        }

        /// <summary>
        /// Sets the properties of the root node based on the provided Neo4j node.
        /// </summary>
        /// <param name="node">The D3Node object representing the root node.</param>
        /// <param name="neoNode">The Neo4j node containing the properties.</param>
        private void SetRootNodeProperties(D3Node node, INode neoNode)
        {
            node.Id = Guid.Parse(neoNode.Properties["uniId"].As<string>());
            node.Name = neoNode.Properties["name"].As<string>();
            node.Type = neoNode.Labels.FirstOrDefault();
            node.NeoId = neoNode.Id.ToString();

            SetNodeAttributes(node, neoNode.Properties);
        }

        /// <summary>
        /// Represents a node in the D3 visualization.
        /// </summary>
        private D3Node CreateD3NodeFromINode(INode neoNode)
        {
            var d3Node = new D3Node
            {
                Id = Guid.Parse(neoNode.Properties["uniId"].As<string>()),
                Name = neoNode.Properties["name"].As<string>(),
                Type = neoNode.Labels.FirstOrDefault(),
                NeoId = neoNode.Id.ToString()
            };

            SetNodeAttributes(d3Node, neoNode.Properties);

            return d3Node;
        }

        /// <summary>
        /// Sets the attributes of a D3Node object based on the provided properties.
        /// </summary>
        /// <param name="d3Node">The D3Node object to set the attributes for.</param>
        /// <param name="properties">The dictionary of properties to set as attributes.</param>
        private void SetNodeAttributes(D3Node d3Node, IReadOnlyDictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                d3Node.Attributes[property.Key] = ConvertPropertyValueToString(property.Value);
            }
        }

        /// <summary>
        /// Converts a property value to its string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string representation of the property value.</returns>
        private string ConvertPropertyValueToString(object value)
        {
            if (value is IEnumerable<object> propertyArray && !(value is string))
            {
                return string.Join(", ", propertyArray);
            }
            return value?.ToString();
        }

        /// <summary>
        /// Sets the relationships between nodes in the D3 graph.
        /// </summary>
        /// <param name="rootNode">The root node of the graph.</param>
        /// <param name="branchNodes">A dictionary of branch nodes.</param>
        /// <param name="relationships">The list of relationships to set.</param>
        private void SetRelationships(D3Node rootNode, Dictionary<string, D3Node> branchNodes, List<IRelationship> relationships)
        {
            foreach (var rel in relationships)
            {
                var startNodeNeoId = rel.StartNodeId.ToString();
                var endNodeNeoId = rel.EndNodeId.ToString();

                // Determine if the relationship connects to the root or another branch node.
                var childNode = branchNodes[startNodeNeoId];
                var parentNode = endNodeNeoId == rootNode.NeoId ? rootNode : branchNodes[endNodeNeoId];

                if (!parentNode.Children.Contains(childNode))
                {
                    parentNode.Children.Add(childNode);
                }
            }
        }

        /// <summary>
        /// Represents a node in the D3 visualization.
        /// </summary>
        private D3Node CreateErrorNode()
        {
            return new D3Node
            {
                Name = "Not Found",
                Type = "Error",
                Id = Guid.Empty
            };
        }
    }
}
