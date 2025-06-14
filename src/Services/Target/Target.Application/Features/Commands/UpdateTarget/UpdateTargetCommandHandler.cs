
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Target.Application.Contracts.Persistence;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using CQRS.Core.Comparators;
using Daikon.Events.Targets;

namespace Target.Application.Features.Command.UpdateTarget
{
    public class UpdateTargetCommandHandler : IRequestHandler<UpdateTargetCommand, Unit>
    {

        private readonly ILogger<UpdateTargetCommandHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        private readonly ITargetRepository _targetRepository;


        public UpdateTargetCommandHandler(ILogger<UpdateTargetCommandHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler,
                                        ITargetRepository targetRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
            _targetRepository = targetRepository;
        }

        public async Task<Unit> Handle(UpdateTargetCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            // check if name is modified; reject if it is
            var existingTarget = await _targetRepository.ReadTargetById(request.Id);
            if (existingTarget.Name != request.Name)
            {
                throw new InvalidOperationException("Name cannot be modified");
            }

            // check if associated genes have been modified; reject if they have, perform a deep comparison
            if (!existingTarget.AssociatedGenes.DictionaryEqual(request.AssociatedGenes))
            {
                throw new InvalidOperationException("Associated genes cannot be modified using this command. Please use the UpdateTargetAssociatedGenesCommand");
            }


            var targetUpdatedEvent = _mapper.Map<TargetUpdatedEvent>(request);
            targetUpdatedEvent.Name = existingTarget.Name;
            targetUpdatedEvent.AssociatedGenes = existingTarget.AssociatedGenes;

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateTarget(targetUpdatedEvent);

                await _eventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(TargetAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}