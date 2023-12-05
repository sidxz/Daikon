
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Features.Command.Gene.AddGene
{
    public class AddGeneToGraphCommandHandler : IRequestHandler<AddGeneToGraphCommand, Unit>
    {
        private readonly ILogger<AddGeneToGraphCommandHandler> _logger;
        private readonly IGraphRepository _graphRepository;

        public AddGeneToGraphCommandHandler(ILogger<AddGeneToGraphCommandHandler> logger, IGraphRepository graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }

        public Task<Unit> Handle(AddGeneToGraphCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _graphRepository.AddGeneToGraph(request.AccessionNumber, request.Name, request.Function, request.Product, request.FunctionalCategory);
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