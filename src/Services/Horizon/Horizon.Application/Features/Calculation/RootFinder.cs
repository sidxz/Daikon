using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistance;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Application.Features.Calculation
{
    public class RootFinder
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<RootFinder> _logger;

        // Constructor initializes the repository and logger.
        public RootFinder(IGraphQueryRepository graphQueryRepository, ILogger<RootFinder> logger)
        {
            _graphQueryRepository = graphQueryRepository ?? throw new ArgumentNullException(nameof(graphQueryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Finds the root node by a unique identifier. First, it tries to find a Gene node, and if not found, any node connected to the identifier.
        public async Task<string> FindByUniqueIdAsync(string uniqueId)
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                throw new ArgumentException("Unique identifier cannot be null or whitespace.", nameof(uniqueId));
            }

            _logger.LogInformation("Starting root search...");

            // Attempt to find a root node connected to the given unique identifier. First, it tries with a Gene node.
            string geneRootQuery = @"MATCH (i {uniId: $uniqueId}) -[*]-> (x:Gene) RETURN x";
            var root = await FindRootNodeAsync(uniqueId, geneRootQuery);
            if (root != null)
            {
                return root;
            }

            // If no Gene node is found, search for any connected node as the root.
            string anyRootQuery = @"MATCH (i {uniId: $uniqueId}) -[*]-> (x) RETURN x";
            root = await FindRootNodeAsync(uniqueId, anyRootQuery);

            // Return the root node's ID or null if no root node is found.
            return root;
        }

        // Helper method to find a root node based on a given Cypher query.
        private async Task<string> FindRootNodeAsync(string uniqueId, string query)
        {
            var parameters = new Dictionary<string, object> { { "uniqueId", uniqueId } };
            var cursor = await _graphQueryRepository.RunAsync(query, parameters);
            var records = await cursor.ToListAsync();
            var rootNode = records.FirstOrDefault();

            if (rootNode != null)
            {
                string rootNodeId = rootNode.Values["x"].As<INode>().Properties["uniId"].As<string>();
                _logger.LogInformation($"Found root node: {rootNodeId}");
                return rootNodeId;
            }

            // Return null if no root node is found.
            return null;
        }
    }
}
