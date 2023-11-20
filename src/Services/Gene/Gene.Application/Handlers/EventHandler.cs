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

        public async Task OnEvent(GeneUpdatedEvent @event)
        {
            var gene = _geneRepository.ReadGeneById(@event.Id).Result;
            gene.Name = @event.Name;
            gene.AccessionNumber = @event.AccessionNumber;
            gene.Function = @event.Function;
            gene.Product = @event.Product;
            gene.FunctionalCategory = @event.FunctionalCategory;
            gene.IsModified = true;
            

            await _geneRepository.UpdateGene(gene);
        }
    }
}