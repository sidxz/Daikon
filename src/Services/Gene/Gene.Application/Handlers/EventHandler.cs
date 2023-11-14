using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.Gene;
using Gene.Application.Contracts.Persistence;

namespace Gene.Application.Query.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IGeneRepository _geneRepository;

        public EventHandler(IGeneRepository geneRepository)
        {
            _geneRepository = geneRepository;
        }

        public async Task OnEvent(GeneCreatedEvent @event)
        {
            var gene = new Domain.Entities.Gene
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

            await _geneRepository.CreateGene(gene);
        }
    }
}