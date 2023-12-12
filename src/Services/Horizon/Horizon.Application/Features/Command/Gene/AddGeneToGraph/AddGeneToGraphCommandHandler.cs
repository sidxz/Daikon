
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Features.Command.Gene.AddGeneToGraph
{
    public class AddGeneToGraphCommandHandler : IRequestHandler<AddGeneToGraphCommand, Unit>
    {
        private readonly ILogger<AddGeneToGraphCommandHandler> _logger;
        private readonly IGraphRepositoryForGene _graphRepositoryForGene;

        public AddGeneToGraphCommandHandler(ILogger<AddGeneToGraphCommandHandler> logger, IGraphRepositoryForGene graphRepositoryForGene)
        {
            _logger = logger;
            _graphRepositoryForGene = graphRepositoryForGene;
        }

        public Task<Unit> Handle(AddGeneToGraphCommand request, CancellationToken cancellationToken)
        {
            var gene = new Domain.Genes.Gene
            {
                GeneId = request.Id.ToString(),
                StrainId = request.StrainId.ToString(),
                AccessionNumber = request.AccessionNumber,
                Name = request.Name,
                Function = request.Function,
                Product = request.Product,
                FunctionalCategory = request.FunctionalCategory,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };
            try
            {   
                //(string geneId, string strainId, string accessionNumber, string name, string function, string product, string functionalCategory
                _graphRepositoryForGene.AddGeneToGraph(gene);
                return Task.FromResult(Unit.Value);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error in AddGeneToGraphCommandHandler");
                throw new Exception("Error in Graph Repository", ex);
            }
        }
    }
}