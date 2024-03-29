
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
            _logger.LogInformation($"Horizon: GeneCreatedEvent: {@event.Id} {@event.AccessionNumber}");
            var gene = new Gene
            {
                GeneId = @event.Id.ToString(),
                StrainId = @event.StrainId.ToString(),

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
                 await _graphRepository.AddGene(gene);
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
                await _graphRepository.AddStrain(strain);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "StrainCreatedEvent Error creating strain", ex);
            }
        }


        public async Task OnEvent(GeneUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: GeneUpdatedEvent: {@event.Id} {@event.AccessionNumber}");
             var gene = new Gene
            {
                GeneId = @event.Id.ToString(),
                StrainId = @event.StrainId.ToString(),

                Name = @event.Name,
                AccessionNumber = @event.AccessionNumber,
                Function = @event.Function,
                Product = @event.Product,
                FunctionalCategory = @event.FunctionalCategory,

                DateCreated = DateTime.UtcNow,
                IsModified = true,
                IsDraft = false
            };

            try {
                 await _graphRepository.UpdateGene(gene);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneCreatedEvent Error creating gene", ex);
            }

        }

        public async Task OnEvent(StrainUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: StrainUpdatedEvent: {@event.Id} {@event.Name}");
            var strain = new Strain
            {
                StrainId = @event.Id.ToString(),
                Name = @event.Name,
                Organism = @event.Organism,
                DateCreated = DateTime.UtcNow,
                IsModified = true,
                IsDraft = false
            };

            try
            {
                await _graphRepository.UpdateStrain(strain);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "StrainCreatedEvent Error creating strain", ex);
            }
        }


        public async Task OnEvent(GeneDeletedEvent @event)
        {
            _logger.LogInformation($"Horizon: GeneDeletedEvent: {@event.Id} {@event.AccessionNumber}");
            try {
                 await _graphRepository.DeleteGene(@event.Id.ToString());
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneDeletedEvent Error deleting gene", ex);
            }
        }
    }
}