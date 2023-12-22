
using AutoMapper;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;

namespace Screen.Application.Features.Commands.RenameScreen
{
    public class RenameScreenCommandHandler : IRequestHandler<RenameScreenCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RenameScreenCommandHandler> _logger;
        private readonly IScreenRepository _screenRepository;

        private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;


        public RenameScreenCommandHandler(ILogger<RenameScreenCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler,
            IScreenRepository screenRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRepository = screenRepository ?? throw new ArgumentNullException(nameof(screenRepository));
            _screenEventSourcingHandler = screenEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenEventSourcingHandler));
        }


        public async Task<Unit> Handle(RenameScreenCommand request, CancellationToken cancellationToken)
        {

            var existingScreen = await _screenRepository.ReadScreenById(request.Id);

            // check if name is available
            var checkScreenName = await _screenRepository.ReadScreenByName(request.Name);
            if (checkScreenName != null)
            {
                throw new DuplicateEntityRequestException(nameof(ScreenAggregate), request.Name);
            }

            var screenRenamedEvent = _mapper.Map<ScreenRenamedEvent>(request);

            try
            {
                var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.RenameScreen(screenRenamedEvent);
                
                await _screenEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(ScreenAggregate), request.Id); ;
            }

            return Unit.Value;

        }
    }
}