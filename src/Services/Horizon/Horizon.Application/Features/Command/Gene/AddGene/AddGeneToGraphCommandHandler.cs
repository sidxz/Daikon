
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Features.Command.Gene.AddGene
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
            try
            {
                _graphRepositoryForGene.AddGeneToGraph(request.AccessionNumber, request.Name, request.Function, request.Product, request.FunctionalCategory);
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