
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Target.Application.Contracts.Persistence;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using CQRS.Core.Comparators;
using Daikon.Events.Targets;

namespace Target.Application.Features.Command.UpdateTargetAssociatedGenes
{
    public class UpdateTargetAssociatedGenesCommandHandler : IRequestHandler<UpdateTargetAssociatedGenesCommand, Unit>
    {

        private readonly ILogger<UpdateTargetAssociatedGenesCommandHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        private readonly ITargetRepository _targetRepository;


        public UpdateTargetAssociatedGenesCommandHandler(ILogger<UpdateTargetAssociatedGenesCommandHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler,
                                        ITargetRepository targetRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
            _targetRepository = targetRepository;
        }

        public async Task<Unit> Handle(UpdateTargetAssociatedGenesCommand request, CancellationToken cancellationToken)
        {

            // check if name is modified; reject if it is
            var target = await _targetRepository.ReadTargetById(request.Id);
            
            // check if associated genes have been modified; reject if they have, perform a deep comparison
            if (target.AssociatedGenes.DictionaryEqual(request.AssociatedGenes))
            {
                throw new InvalidOperationException("No changes to associated genes detected.");
            }

            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

                var targetUpdatedEvent = _mapper.Map<TargetAssociatedGenesUpdatedEvent>(request);

                aggregate.UpdateTargetAssociatedGenes(targetUpdatedEvent);

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