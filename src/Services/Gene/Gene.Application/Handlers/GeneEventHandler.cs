
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.Handlers
{
    public class GeneEventHandler : IGeneEventHandler
    {
        private readonly IGeneRepository _geneRepository;
        private readonly IGeneEssentialityRepository _geneEssentialityRepository;

        private readonly ILogger<GeneEventHandler> _logger;

        public GeneEventHandler(IGeneRepository geneRepository, IGeneEssentialityRepository geneEssentialityRepository, ILogger<GeneEventHandler> logger)
        {
            _geneRepository = geneRepository ?? throw new ArgumentNullException(nameof(geneRepository));
            _geneEssentialityRepository = geneEssentialityRepository ?? throw new ArgumentNullException(nameof(geneEssentialityRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnEvent(GeneCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneCreatedEvent: {Id}", @event.Id);
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

            try
            {
                await _geneRepository.CreateGene(gene);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneCreatedEvent Error creating gene", ex);
            }
        }


        public async Task OnEvent(GeneUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneUpdatedEvent: {Id}", @event.Id);
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
            _logger.LogInformation("OnEvent: GeneDeletedEvent: {Id}", @event.Id);
            var gene = _geneRepository.ReadGeneById(@event.Id).Result;

            try
            {
                await _geneEssentialityRepository.DeleteAllEssentialitiesOfGene(gene.Id);
                await _geneRepository.DeleteGene(gene.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneDeletedEvent Error deleting essentialities of gene with id @event.Id", ex);
            }

        }

        public async Task OnEvent(GeneEssentialityAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneEssentialityAddedEvent: {EssentialityId}", @event.EssentialityId);
            var essentiality = new Domain.Entities.Essentiality
            {
                Id = @event.EssentialityId,
                GeneId = @event.GeneId,
                EssentialityId = @event.EssentialityId,
                Classification = @event.Classification,
                Condition = @event.Condition,
                Method = @event.Method,
                Reference = @event.Reference,
                Note = @event.Note,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            try
            {
                await _geneEssentialityRepository.AddEssentiality(essentiality);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneEssentialityCreatedEvent Error creating essentiality", ex);
            }
        }

        public async Task OnEvent(GeneEssentialityUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneEssentialityUpdatedEvent: {EssentialityId}", @event.EssentialityId);

            var essentiality = _geneEssentialityRepository.Read(@event.EssentialityId).Result;

            essentiality.Classification = @event.Classification;
            essentiality.Condition = @event.Condition;
            essentiality.Method = @event.Method;
            essentiality.Reference = @event.Reference;
            essentiality.Note = @event.Note;
            essentiality.IsModified = true;

            try
            {
                await _geneEssentialityRepository.UpdateEssentiality(essentiality);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneEssentialityUpdatedEvent Error updating essentiality with id @event.EssentialityId", ex);
            }
        }

        public async Task OnEvent(GeneEssentialityDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneEssentialityDeletedEvent: {EssentialityId}", @event.EssentialityId);
            try
            {
                await _geneEssentialityRepository.DeleteEssentiality(@event.EssentialityId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneEssentialityDeletedEvent Error deleting essentiality with id @event.EssentialityId", ex);
            }
        }
    }
}