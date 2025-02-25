
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Features.Command.Gene.AddGene
{
    public class AddGeneCommandHandler : IRequestHandler<AddGeneCommand, Unit>
    {
        private readonly ILogger<AddGeneCommandHandler> _logger;
        private readonly IGraphRepositoryForGene _graphRepositoryForGene;

        public AddGeneCommandHandler(ILogger<AddGeneCommandHandler> logger, IGraphRepositoryForGene graphRepositoryForGene)
        {
            _logger = logger;
            _graphRepositoryForGene = graphRepositoryForGene;
        }

        public Task<Unit> Handle(AddGeneCommand request, CancellationToken cancellationToken)
        {
            var gene = new Domain.Genes.Gene
            {
                GeneId = request.Id.ToString(),
                StrainId = request.StrainId.ToString(),
                AccessionNumber = request.AccessionNumber,
                Name = request.Name,
                Product = request.Product,
                FunctionalCategory = request.FunctionalCategory,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };
            try
            {   
                //(string geneId, string strainId, string accessionNumber, string name, string function, string product, string functionalCategory
                _graphRepositoryForGene.AddGene(gene);
                return Task.FromResult(Unit.Value);
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Error in AddGeneCommandHandler");
                throw new Exception("Error in Graph Repository", ex);
            }
        }
    }
}