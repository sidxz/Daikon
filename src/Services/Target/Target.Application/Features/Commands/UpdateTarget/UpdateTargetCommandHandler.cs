
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Target.Application.Contracts.Persistence;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Target.Application.Features.Command.UpdateTarget
{
    public class UpdateTargetCommandHandler : IRequestHandler<UpdateTargetCommand, Unit>
    {

        private readonly ILogger<UpdateTargetCommandHandler> _logger;
        private readonly IMapper _mapper;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        private readonly ITargetRepository _targetRepository;
        

        public UpdateTargetCommandHandler(ILogger<UpdateTargetCommandHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler,
                                        ITargetRepository geneRepository,  IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _eventSourcingHandler = eventSourcingHandler;
            _targetRepository = geneRepository;
        }

        public async Task<Unit> Handle(UpdateTargetCommand request, CancellationToken cancellationToken)
        {

            // check if name is modified; reject if it is
            var existingTarget = await _targetRepository.ReadTargetById(request.Id);
            if (existingTarget.Name != request.Name)
            {
                throw new InvalidOperationException("Name cannot be modified");
            }

        
            var target = _mapper.Map<Domain.Entities.Target>(request);

            // Things that cannot be modified
            target.Name = existingTarget.Name;            
            try
            {
                var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateTarget(target, _mapper);

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