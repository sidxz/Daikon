
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Daikon.Events.Strains;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Genes;
using Horizon.Domain.Strains;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Query.Handlers
{
    public class GeneEventHandler : IGeneEventHandler
    {
        private readonly ILogger<GeneEventHandler> _logger;
        private readonly IGraphRepositoryForGene _graphRepository;

        public GeneEventHandler(ILogger<GeneEventHandler> logger, IGraphRepositoryForGene graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }

        public async Task OnEvent(GeneCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: GeneCreatedEvent: {@event.Id} {@event.Name}");
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

        public async Task OnEvent(StrainCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: StrainCreatedEvent: {@event.Id} {@event.Name}");
            var strain = new Strain
            {
                StrainId = @event.Id.ToString(),
                Name = @event.Name,
                Organism = @event.Organism,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            try
            {
                await _graphRepository.AddStrainToGraph(strain.Name, strain.StrainId, strain.Organism);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "StrainCreatedEvent Error creating strain", ex);
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