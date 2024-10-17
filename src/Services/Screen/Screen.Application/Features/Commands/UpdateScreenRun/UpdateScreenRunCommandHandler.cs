using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;


namespace Screen.Application.Features.Commands.UpdateScreenRun
{
    public class UpdateScreenRunCommandHandler : IRequestHandler<UpdateScreenRunCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateScreenRunCommandHandler> _logger;
        private readonly IScreenRunRepository _screenRunRepository;

        private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;

        public UpdateScreenRunCommandHandler(ILogger<UpdateScreenRunCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler,
            IScreenRunRepository screenRunRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRunRepository = screenRunRepository ?? throw new ArgumentNullException(nameof(screenRunRepository));
            _screenEventSourcingHandler = screenEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateScreenRunCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);

            var screenRunUpdatedEvent = _mapper.Map<ScreenRunUpdatedEvent>(request);
            screenRunUpdatedEvent.LastModifiedById = request.RequestorUserId;

            try
            {
                var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateScreenRun(screenRunUpdatedEvent);

                await _screenEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(ScreenAggregate), request.Id);
            }
            return Unit.Value;
        }

    }
}