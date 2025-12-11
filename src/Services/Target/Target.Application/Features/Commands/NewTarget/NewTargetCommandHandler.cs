
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using Daikon.Events.Targets;
using Target.Domain.Services;

namespace Target.Application.Features.Command.NewTarget
{
    public class NewTargetCommandHandler : IRequestHandler<NewTargetCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewTargetCommandHandler> _logger;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        private readonly ITargetUniquenessChecker _targetUniquenessChecker;

        public NewTargetCommandHandler(ILogger<NewTargetCommandHandler> logger,
            IEventSourcingHandler<TargetAggregate> eventSourcingHandler,
            IMapper mapper, ITargetUniquenessChecker targetUniquenessChecker)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
            _targetUniquenessChecker = targetUniquenessChecker ?? throw new ArgumentNullException(nameof(targetUniquenessChecker));
        }


        public async Task<Unit> Handle(NewTargetCommand request, CancellationToken cancellationToken)
        {

            request.SetCreateProperties(request.RequestorUserId);
            try
            {
                var targetCreatedEvent = _mapper.Map<TargetCreatedEvent>(request);

                var aggregate = await TargetAggregate.CreateAsync(targetCreatedEvent, _targetUniquenessChecker);

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