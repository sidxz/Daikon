
using CQRS.Core.Exceptions;
using Daikon.Events.Strains;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Entities;

namespace Gene.Application.Query.EventHandlers
{
    public class StrainEventHandler : IStrainEventHandler
    {
        private readonly IStrainRepository _strainRepository;

        public StrainEventHandler(IStrainRepository strainRepository)
        {
            _strainRepository = strainRepository;
        }

        public async Task OnEvent(StrainCreatedEvent @event)
        {
            var strain = new Strain
            {
                Id = @event.Id,
                Name = @event.Name,
                Organism = @event.Organism,
                IsModified = false,
                IsDraft = false
            };

            try {
                await _strainRepository.CreateStrain(strain);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "StrainCreatedEvent Error creating strain", ex);
            }
        }


        public async Task OnEvent(StrainUpdatedEvent @event)
        {
            var strain = await _strainRepository.ReadStrainById(@event.Id);

            strain.Name = @event.Name;
            strain.Organism = @event.Organism;
            strain.IsModified = true;
            // Preserve the original creation date and creator
            strain.CreatedById = strain.CreatedById;
            strain.DateCreated = strain.DateCreated;
            

            try
            {
                await _strainRepository.UpdateStrain(strain);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "StrainUpdatedEvent Error updating strain with id @event.Id", ex);
            }

        }


        public async Task OnEvent(StrainDeletedEvent @event)
        {
            var strain = await _strainRepository.ReadStrainById(@event.Id);

            try
            {
                await _strainRepository.DeleteStrain(strain.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "StrainDeletedEvent Error deleting strain with id @event.Id", ex);
            }
        }
    }
}