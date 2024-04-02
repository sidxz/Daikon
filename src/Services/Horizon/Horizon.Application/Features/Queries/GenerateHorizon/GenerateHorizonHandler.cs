using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistance;
using Horizon.Application.VMs.D3;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using Neo4j.Driver;

namespace Horizon.Application.Features.Queries.GenerateHorizon
{
    public class GenerateHorizonHandler : IRequestHandler<GenerateHorizonQuery, GenerateHorizonResponseVM>
    {
        private readonly IGraphQueryRepository _graphQueryRepository;
        private readonly ILogger<GenerateHorizonHandler> _logger;

        public GenerateHorizonHandler(IGraphQueryRepository graphQueryRepository, ILogger<GenerateHorizonHandler> logger)
        {
            _graphQueryRepository = graphQueryRepository;
            _logger = logger;
        }

        public async Task<GenerateHorizonResponseVM> Handle(GenerateHorizonQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("=======================================================START=======================================================");


            var runQuery = @"MATCH (i:Gene {geneId : $_geneId}) <-[rel*]-(x)
                                    WHERE x:Gene OR x:Target OR x:Screen OR x:HitCollection
                                    RETURN i, rel, x";
            var parameters = new Dictionary<string, object> { { "_geneId", "03ed5d27-2750-4d3e-ba36-286461188595" } };

            /* 
             * i = index node
             * rel = relationship
             * x = branch node
             */

            var cursor = await _graphQueryRepository.RunAsync(runQuery, parameters);
            var records = await cursor.ToListAsync(cancellationToken: cancellationToken);

            var recordsJson = System.Text.Json.JsonSerializer.Serialize(records);
            //Console.WriteLine(recordsJson);


            // parse the index and save it as parent.
            var rootNode = new D3Node();
            _logger.LogInformation($"Setting Root Node....");
            foreach (var record in records)
            {
                var node = record["i"].As<INode>();

                rootNode.Name = node["name"].As<string>();
                rootNode.Type = node.Labels.First();
                rootNode.NeoId = node.ElementId.ToString();
                rootNode.Attributes = new Dictionary<string, string>
                {
                    { "accessionNumber", node["accessionNumber"].As<string>() },
                    { "geneId", node["geneId"].As<string>() },
                };

                //_logger.LogInformation($"Node: {System.Text.Json.JsonSerializer.Serialize(node)}");
                break;
            }
            _logger.LogInformation($"Root Node: {System.Text.Json.JsonSerializer.Serialize(rootNode)}");


            // parse the branches and save them in a dictionary, index by their neoId.
            var branchNodes = new Dictionary<string, D3Node>();
            _logger.LogInformation($"Setting Branch Nodes....");
            foreach (var record in records)
            {
                var node = record["x"].As<INode>();

                var branchNode = new D3Node
                {
                    Name = node["name"].As<string>(),
                    Type = node.Labels.First(),
                    NeoId = node.ElementId.ToString(),


                };

                branchNodes.Add(branchNode.NeoId, branchNode);
                //_logger.LogInformation($"Node: {System.Text.Json.JsonSerializer.Serialize(node)}");
            }

            _logger.LogInformation($"Branch Nodes: {System.Text.Json.JsonSerializer.Serialize(branchNodes)}");

            // parse the relationships and connect the nodes.
            _logger.LogInformation($"Setting Relationships....");
            foreach (var record in records)
            {
                var relationships = record["rel"].As<List<IRelationship>>();

                foreach (var rel in relationships)
                {
                    var startNodeId = rel.StartNodeElementId;
                    var endNodeId = rel.EndNodeElementId;

                    _logger.LogInformation($"Relationship: {startNodeId} -> {endNodeId}");

                    // check if the endNodeId is the index node, if so, set it as child of the index node.
                    if (endNodeId == rootNode.NeoId)
                    {
                        // check if the startNodeId is already a child of the index node.
                        if (rootNode.Children.Any(x => x.NeoId == branchNodes[startNodeId].NeoId))
                        {
                            _logger.LogInformation($"Child already exists in root: {branchNodes[startNodeId].Name}");
                            continue;
                        }
                        _logger.LogInformation($"Adding child to root: {branchNodes[startNodeId].Name}");
                        rootNode.Children.Add(branchNodes[startNodeId]);
                        continue;
                    }
                    else
                    {
                        _logger.LogInformation($"Parent {branchNodes[endNodeId].Name} Child --> : {branchNodes[startNodeId].Name}");
                        var parent = branchNodes[endNodeId];
                        // check if the startNodeId is already a child of the parent node.
                        if (parent.Children.Any(x => x.NeoId == branchNodes[startNodeId].NeoId))
                        {
                            _logger.LogInformation($"Child already exists in parent: {branchNodes[startNodeId].Name}");
                            continue;
                        }
                        parent.Children.Add(branchNodes[startNodeId]);
                    }

                }
                //_logger.LogInformation($"Relationship: {System.Text.Json.JsonSerializer.Serialize(rel)}");
            }

            _logger.LogInformation($"Final Tree Node: {System.Text.Json.JsonSerializer.Serialize(rootNode)}");

            var resp = new GenerateHorizonResponseVM();

            _logger.LogInformation("=======================================================END=======================================================");
            return resp;
        }
    }
}