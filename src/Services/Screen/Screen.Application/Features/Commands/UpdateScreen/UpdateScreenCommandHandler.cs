
using AutoMapper;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using CQRS.Core.Comparators;
using CQRS.Core.Exceptions;

namespace Screen.Application.Features.Commands.UpdateScreen
{
    public class UpdateScreenCommandHandler : IRequestHandler<UpdateScreenCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateScreenCommandHandler> _logger;
        private readonly IScreenRepository _screenRepository;

        private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;


        public UpdateScreenCommandHandler(ILogger<UpdateScreenCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler,
            IScreenRepository screenRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRepository = screenRepository ?? throw new ArgumentNullException(nameof(screenRepository));
            _screenEventSourcingHandler = screenEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenEventSourcingHandler));
        }


        public async Task<Unit> Handle(UpdateScreenCommand request, CancellationToken cancellationToken)
        {
            // check if name is modified; reject if it is
            var existingScreen = await _screenRepository.ReadScreenById(request.Id);
            if (existingScreen.Name != request.Name)
            {
                throw new InvalidOperationException("Name cannot be modified");
            }

            // check if associated targets have been modified; reject if they have, perform a deep comparison
            if (!existingScreen.AssociatedTargets.DictionaryEqual(request.AssociatedTargets))
            {
                throw new InvalidOperationException("Associated targets cannot be modified using this command. Please use the UpdateScreenAssociatedTargetsCommand");
            }

            var modScreen = _mapper.Map<Domain.Entities.Screen>(request);
            modScreen.Name = existingScreen.Name;

            try {
                var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateScreen(modScreen);

                await _screenEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(ScreenAggregate), request.Id);;
            }

            return Unit.Value;

        }
    }
}