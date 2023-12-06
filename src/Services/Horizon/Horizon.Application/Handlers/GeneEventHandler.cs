
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Genes;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Query.Handlers
{
    public class GeneEventHandler : IGeneEventHandler
    {
        private readonly ILogger<GeneEventHandler> _logger;
        private readonly IGraphRepository _graphRepository;

        public GeneEventHandler(ILogger<GeneEventHandler> logger, IGraphRepository graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }

        public async Task OnEvent(GeneCreatedEvent @event)
        {
            var gene = new Gene
            {
                Id = @event.Id,
                Name = @event.Name,

                AccessionNumber = @event.AccessionNumber,
                Function = @event.Function,
                Product = @event.Product,
                FunctionalCategory = @event.FunctionalCategory,

                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            try {
                 await _graphRepository.AddGeneToGraph(gene.AccessionNumber, gene.Name, gene.Function, gene.Product, gene.FunctionalCategory);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneCreatedEvent Error creating gene", ex);
            }
        }


        public async Task OnEvent(GeneUpdatedEvent @event)
        {
            throw new NotImplementedException();

        }


        public async Task OnEvent(GeneDeletedEvent @event)
        {
            throw new NotImplementedException();

        }
    }
}