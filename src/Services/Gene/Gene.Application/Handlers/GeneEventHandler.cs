
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;

namespace Gene.Application.Query.Handlers
{
    public class GeneEventHandler : IGeneEventHandler
    {
        private readonly IGeneRepository _geneRepository;

        public GeneEventHandler(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository;
        }

        public async Task OnEvent(GeneCreatedEvent @event)
        {
            var gene = new Domain.Entities.Gene
            {
                Id = @event.Id,
                StrainId = @event.StrainId,
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
                await _geneRepository.CreateGene(gene);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneCreatedEvent Error creating gene", ex);
            }
        }


        public async Task OnEvent(GeneUpdatedEvent @event)
        {
            var gene = _geneRepository.ReadGeneById(@event.Id).Result;

            gene.StrainId = @event.StrainId;
            gene.Name = @event.Name;
            gene.AccessionNumber = @event.AccessionNumber;
            gene.Function = @event.Function;
            gene.Product = @event.Product;
            gene.FunctionalCategory = @event.FunctionalCategory;
            gene.IsModified = true;

            try
            {
                await _geneRepository.UpdateGene(gene);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneUpdatedEvent Error updating gene with id @event.Id", ex);
            }

        }


        public async Task OnEvent(GeneDeletedEvent @event)
        {
            var gene = _geneRepository.ReadGeneById(@event.Id).Result;

            try
            {
                await _geneRepository.DeleteGene(gene.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneDeletedEvent Error deleting gene with id @event.Id", ex);
            }
        }
    }
}