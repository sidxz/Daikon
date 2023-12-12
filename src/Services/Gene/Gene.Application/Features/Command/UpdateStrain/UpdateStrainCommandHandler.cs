using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Gene.Application.Features.Command.UpdateStrain
{
    public class UpdateStrainCommandHandler : IRequestHandler<UpdateStrainCommand, Unit>
    {

        private readonly ILogger<UpdateStrainCommandHandler> _logger;

        private readonly IEventSourcingHandler<StrainAggregate> _eventSourcingHandler;

        public UpdateStrainCommandHandler(ILogger<UpdateStrainCommandHandler> logger, IEventSourcingHandler<StrainAggregate> eventSourcingHandler)
        {
            _logger = logger;
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task<Unit> Handle(UpdateStrainCommand request, CancellationToken cancellationToken)
        {
            var strain = new Domain.Entities.Strain
            {
                Id = request.Id,
                Name = request.Name,
                Organism = request.Organism,
            };

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateStrain(strain);
                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}