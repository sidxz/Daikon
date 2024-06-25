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

            // See if node is of type gene, then send it as root
            _logger.LogInformation("Checking if node is of type gene...");
            string geneRootQuery = @"MATCH (x:Gene {uniId: $uniqueId}) RETURN x";
            var node = await FindNodeAsync(uniqueId, geneRootQuery);
            if (node != null)
            {
                // This is a gene node, so return it as root
                _logger.LogInformation("Node is of type gene.");
                return node.Values["x"].As<INode>().Properties["uniId"].As<string>();
            }

            // Attempt to find if it is a target node by relation query
            _logger.LogInformation("Checking if it is a target node by relation query...");
            string targetRootQuery = @"MATCH (i {uniId: $uniqueId}) -[*]-> (x:Target) RETURN x";
            node = await FindNodeAsync(uniqueId, targetRootQuery);

            if (node == null)
            {
                _logger.LogInformation("target node by relation query returned NULL...");
                // Cross check if it is a target node
                _logger.LogInformation("Checking if it is a target node by direct query...");
                string targetQuery = @"MATCH (x:Target {uniId: $uniqueId}) RETURN x";
                node = await FindNodeAsync(uniqueId, targetQuery);
            }

            if (node != null)
            {
                // This is a target node. Check count of associated genes
                _logger.LogInformation("Node is of type target.");

                var associatedGenes = node.Values["x"].As<INode>().Properties["associatedGenes"];
                _logger.LogInformation($"Associated genes: {associatedGenes}");

                if (associatedGenes != null && associatedGenes.As<List<string>>().Count == 1)
                {
                    // Only one associated gene, so return it as root
                    _logger.LogInformation("-> Only one associated gene, so returning it as root.");
                    var accessionNo = associatedGenes.As<List<string>>()[0];
                    _logger.LogInformation($"Accession number: {accessionNo}");
                    var geneQuery = @"MATCH (x:Gene {accessionNumber: $accessionNumber}) RETURN x";
                    var geneNode = await FindGeneAsync(accessionNo, geneQuery);
                    if (geneNode != null)
                    {
                        _logger.LogInformation("Returning gene with id {geneId} as root.", geneNode.Values["x"].As<INode>().Properties["uniId"].As<string>());
                        return geneNode.Values["x"].As<INode>().Properties["uniId"].As<string>();
                    }
                }
                if (associatedGenes != null && associatedGenes.As<List<string>>().Count >= 1)
                {
                    // More than one associated gene, so returning any as root.
                    _logger.LogInformation("-> More than one associated gene, so returning any as root.");
                    var accessionNo = associatedGenes.As<List<string>>()[0];
                    _logger.LogInformation($"Accession number: {accessionNo}");
                    var geneQuery = @"MATCH (x:Gene {accessionNumber: $accessionNumber}) RETURN x";
                    var geneNode = await FindGeneAsync(accessionNo, geneQuery);
                    if (geneNode != null)
                    {
                        _logger.LogInformation("Returning gene with id {geneId} as root.", geneNode.Values["x"].As<INode>().Properties["uniId"].As<string>());
                        return geneNode.Values["x"].As<INode>().Properties["uniId"].As<string>();
                    }
                }

                // More than one associated gene or no associated genes, so return the target node as root
                _logger.LogInformation("More than one associated gene or no associated genes, so returning target node as root.");
                return node.Values["x"].As<INode>().Properties["uniId"].As<string>();
            }

            // At this point, root is NOT a gene or target node. So, search for any valid connected node as the root.
            _logger.LogInformation("Search for any connected node as the root....");

            string anyRootQuery = @"MATCH (i {uniId: $uniqueId}) -[*]-> (x) 
            WHERE x:Gene OR x:Target OR x:Screen OR x:HitAssessment OR x:Project
            RETURN x";
            node = await FindLastNodeAsync(uniqueId, anyRootQuery);
            if (node != null)
            {
                // This is a connected node, so return it as root
                _logger.LogInformation("Connected node found, so returning it as root.");
                return node.Values["x"].As<INode>().Properties["uniId"].As<string>();
            }

            // At this point, the node is itself a root node, or the node does not exist in db.
            _logger.LogInformation("Node is itself a root node, or the node does not exist in db.");
            var rootQuery = @"MATCH (x {uniId: $uniqueId}) RETURN x";
            node = await FindNodeAsync(uniqueId, rootQuery);

            if (node != null)
            {
                // This is a root node, so return it as root
                _logger.LogInformation("Node is itself a root node.");
                return node.Values["x"].As<INode>().Properties["uniId"].As<string>();
            }

            // Return the root node's ID or null if no root node is found.
            _logger.LogInformation("Root node not found.");
            return null;
        }

        // Helper method to find a root node based on a given Cypher query.
        private async Task<IRecord> FindNodeAsync(string uniqueId, string query)
        {
            var parameters = new Dictionary<string, object> { { "uniqueId", uniqueId } };
            var cursor = await _graphQueryRepository.RunAsync(query, parameters);
            var records = await cursor.ToListAsync();
            var firstNode = records.FirstOrDefault();

            if (firstNode != null)
            {
                return firstNode;
            }

            // Return null if no root node is found.
            return null;
        }
        private async Task<IRecord> FindLastNodeAsync(string uniqueId, string query)
        {
            var parameters = new Dictionary<string, object> { { "uniqueId", uniqueId } };
            var cursor = await _graphQueryRepository.RunAsync(query, parameters);
            var records = await cursor.ToListAsync();
            var lastNode = records.LastOrDefault();

            if (lastNode != null)
            {
                return lastNode;
            }

            // Return null if no root node is found.
            return null;
        }

        private async Task<IRecord> FindGeneAsync(string accessionNumber, string query)
        {
            var parameters = new Dictionary<string, object> { { "accessionNumber", accessionNumber } };
            var cursor = await _graphQueryRepository.RunAsync(query, parameters);
            var records = await cursor.ToListAsync();
            var firstNode = records.FirstOrDefault();

            if (firstNode != null)
            {
                return firstNode;
            }
            return null;
        }
    }
}
