using AutoMapper;
using CQRS.Core.Domain;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;

namespace Screen.Application.Features.Commands.NewScreen
{
    public class NewScreenCommandHandler : IRequestHandler<NewScreenCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewScreenCommandHandler> _logger;
        private readonly IScreenRepository _screenRepository;
        private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;

        public NewScreenCommandHandler(ILogger<NewScreenCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler,
            IScreenRepository screenRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRepository = screenRepository ?? throw new ArgumentNullException(nameof(screenRepository));
            _screenEventSourcingHandler = screenEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenEventSourcingHandler));
        }

        public async Task<Unit> Handle(NewScreenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.SetCreateProperties(request.RequestorUserId);
                _logger.LogInformation($"Handling NewScreenCommand: {request}");

                // check if name exists
                var existingScreen = await _screenRepository.ReadScreenByName(request.Name);
                if (existingScreen != null)
                {
                    _logger.LogWarning("Screen name already exists: {Name}", request.Name);
                    throw new InvalidOperationException("Screen name already exists");
                }

                var newScreenCreatedEvent = _mapper.Map<ScreenCreatedEvent>(request);

                newScreenCreatedEvent.LatestStatusChangeDate = new DVariable<DateTime>(DateTime.UtcNow);
                var aggregate = new ScreenAggregate(newScreenCreatedEvent);

                await _screenEventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling NewScreenCommand");
                throw;
            }
        }
    }
}
