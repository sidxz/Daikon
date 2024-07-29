using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Targets;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Features.Commands.AddToxicology;
using Target.Application.Features.Commands.UpdateToxicology;
using Target.Domain.Aggregates;

namespace Target.Application.Features.Commands.AddOrUpdateToxicology
{
    public class AddOrUpdateToxicologyHandler(IMediator _mediator, IMapper mapper, ILogger<AddOrUpdateToxicologyHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler)
     : IRequestHandler<AddOrUpdateToxicologyCommand, Unit>
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<AddOrUpdateToxicologyHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));

        private readonly IMediator _mediator = _mediator ?? throw new ArgumentNullException(nameof(_mediator));
        public async Task<Unit> Handle(AddOrUpdateToxicologyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddOrUpdateToxicologyCommand {request}", request);

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                // find if the topic exists
                var existingToxicology = aggregate._toxicology.FirstOrDefault(x => x.Value.Topic == request.Topic).Value;
                if (existingToxicology != null)
                {
                    _logger.LogInformation("Toxicology already exists. Updating toxicology");
                    // check if toxicology has changed
                    // convert to json and pretty print existing and request to debug, use system.text.json
                    // var existingToxicologyJson = JsonSerializer.Serialize(existingToxicology, new JsonSerializerOptions { WriteIndented = true });
                    // var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true }); 
                    // _logger.LogDebug("Existing Toxicology: {existingToxicologyJson}", existingToxicologyJson);
                    // _logger.LogDebug("Request: {requestJson}", requestJson);


                    if (existingToxicology.Impact.Value == request.Impact.Value
                        && existingToxicology.ImpactPriority.Value == request.ImpactPriority.Value
                        && existingToxicology.Likelihood.Value == request.Likelihood.Value
                        && existingToxicology.LikelihoodPriority.Value == request.LikelihoodPriority.Value
                        && existingToxicology.Note.Value == request.Note.Value)
                    {
                        _logger.LogInformation("Toxicology has not changed. No need to update toxicology");
                        return Unit.Value;
                    }
                    // update the existing toxicology
                    request.DateModified = DateTime.UtcNow;
                    request.IsModified = true;
                    var toxicologyUpdatedCommand = _mapper.Map<UpdateToxicologyCommand>(request);
                    toxicologyUpdatedCommand.Id = request.Id;
                    toxicologyUpdatedCommand.TargetId = request.Id;
                    toxicologyUpdatedCommand.ToxicologyId = existingToxicology.ToxicologyId;
                    await _mediator.Send(toxicologyUpdatedCommand, cancellationToken);
                    return Unit.Value;
                }

                else
                {
                    _logger.LogInformation("Toxicology does not exist. Adding toxicology");
                    request.DateCreated = DateTime.UtcNow;
                    request.IsModified = false;
                    var toxicologyAddCommand = _mapper.Map<AddToxicologyCommand>(request);
                    var toxicologyId = Guid.NewGuid();
                    toxicologyAddCommand.Id = request.Id;
                    toxicologyAddCommand.TargetId = request.Id;
                    toxicologyAddCommand.ToxicologyId = toxicologyId;

                    await _mediator.Send(toxicologyAddCommand, cancellationToken);
                    return Unit.Value;
                }

            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(TargetAggregate), request.Id);
            }

        }
    }
}