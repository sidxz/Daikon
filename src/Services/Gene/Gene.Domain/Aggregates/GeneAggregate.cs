using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Daikon.Events.Gene;

namespace Gene.Domain.Aggregates
{
    public class GeneAggregate : AggregateRoot
    {

        public GeneAggregate()
        {

        }

        public GeneAggregate(Entities.Gene gene)
        {
            RaiseEvent(new GeneCreatedEvent
            {
                Name = gene.Name,
                Id = gene.Id,
                
                AccessionNumber = gene.AccessionNumber,
                Function = gene.Function,
                Product = gene.Product,
                FunctionalCategory = gene.FunctionalCategory,
                
                DateCreated = DateTime.UtcNow
            });
        }

        public void Apply(GeneCreatedEvent @event)
        {
            _id = @event.Id;           
        }
    }
}