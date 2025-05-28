
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Daikon.Events.Targets;

namespace Target.Application.Features.Command.NewTarget
{
    public class NewTargetCommandHandler : IRequestHandler<NewTargetCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewTargetCommandHandler> _logger;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        private readonly ITargetRepository _targetRepository;

        public NewTargetCommandHandler(ILogger<NewTargetCommandHandler> logger,
            IEventSourcingHandler<TargetAggregate> eventSourcingHandler,
            IMapper mapper, ITargetRepository targetRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _targetRepository = targetRepository ?? throw new ArgumentNullException(nameof(targetRepository));
        }


        public async Task<Unit> Handle(NewTargetCommand request, CancellationToken cancellationToken)
        {

            request.SetCreateProperties(request.RequestorUserId);
            // check if target (targetName) already exists within same strain ; reject if it does
            var existingTarget = await _targetRepository.ReadTargetByName(request.Name);
            if (existingTarget!= null && (existingTarget.Name == request.Name && existingTarget.StrainId == request.StrainId))
            {
                throw new DuplicateEntityRequestException(nameof(NewTargetCommand), request.Name);
            }

            try
            {
                var targetCreatedEvent = _mapper.Map<TargetCreatedEvent>(request);

                var aggregate = new TargetAggregate(targetCreatedEvent);

                await _eventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the target with Name {TargetName}", request.Name);
                throw;
            }

        }
    }

}