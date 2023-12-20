
using AutoMapper;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using CQRS.Core.Comparators;
using CQRS.Core.Exceptions;

namespace Screen.Application.Features.Commands.UpdateScreenAssociatedTargets
{
    public class UpdateScreenAssociatedTargetsCommandHandler : IRequestHandler<UpdateScreenAssociatedTargetsCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateScreenAssociatedTargetsCommandHandler> _logger;
        private readonly IScreenRepository _screenRepository;
        private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;


        public UpdateScreenAssociatedTargetsCommandHandler(ILogger<UpdateScreenAssociatedTargetsCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler,
            IScreenRepository screenRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRepository = screenRepository ?? throw new ArgumentNullException(nameof(screenRepository));
            _screenEventSourcingHandler = screenEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenEventSourcingHandler));
        }
        public async Task<Unit> Handle(UpdateScreenAssociatedTargetsCommand request, CancellationToken cancellationToken)
        {
            // check if name is modified; reject if it is
            var screen = await _screenRepository.ReadScreenById(request.Id);


            // check if associated targets have been modified; reject if not, perform a deep comparison
            if (screen.AssociatedTargets.DictionaryEqual(request.AssociatedTargets))
            {
                throw new InvalidOperationException("Associated targets are not modified");
            }

            screen.AssociatedTargets = request.AssociatedTargets;

            try
            {
                var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateScreenAssociatedTargets(request.AssociatedTargets);

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