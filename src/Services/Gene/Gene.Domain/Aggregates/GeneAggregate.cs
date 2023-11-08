using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Daikon.Events.Gene;
using Gene.Domain.Models;

namespace Gene.Domain.Aggregates
{
    public class GeneAggregate : AggregateRoot
    {

        public GeneAggregate()
        {

        }

        public GeneAggregate(GeneModel gene)
        {
            RaiseEvent(new GeneCreatedEvent
            {
                Name = gene.Name,
                Id = gene.Id,
                StrainId = gene.StrainId,
                AccessionNumber = gene.AccessionNumber,
                Function = gene.Function,
                Product = gene.Product,
                FunctionalCategory = gene.FunctionalCategory,
                ExternalIds = gene.ExternalIds,
                DateCreated = DateTime.UtcNow
            });
        }

        public void Apply(GeneCreatedEvent @event)
        {
            _id = @event.Id;           
        }
    }
}