using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Gene.Application.Features.Command.UpdateGene
{
    public class UpdateGeneCommandHandler : IRequestHandler<UpdateGeneCommand, Unit>
    {

        private readonly ILogger<UpdateGeneCommandHandler> _logger;

        private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

        public UpdateGeneCommandHandler(ILogger<UpdateGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task<Unit> Handle(UpdateGeneCommand request, CancellationToken cancellationToken)
        {
           var gene = new Domain.Entities.Gene{
                Id = request.Id,
                AccessionNumber = request.AccessionNumber,
                Name = request.Name,
                Function = request.Function,
                Product = request.Product,
                FunctionalCategory = request.FunctionalCategory
            };
            
            var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
            // Print the aggregate in json format
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = aggregate.ToJson(jsonWriterSettings);
            
            _logger.LogInformation($"++++++++Aggregate: {json}");

            aggregate.UpdateGene(gene);
            await _eventSourcingHandler.SaveAsync(aggregate);

            return Unit.Value;
        }
    }
}