
using AutoMapper;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using CQRS.Core.Exceptions;
using Daikon.Events.Screens;

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
            request.SetUpdateProperties(request.RequestorUserId);
            
            var screenAssociatedTargetsUpdatedEvent = _mapper.Map<ScreenAssociatedTargetsUpdatedEvent>(request);
            
            try
            {
                var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.UpdateScreenAssociatedTargets(screenAssociatedTargetsUpdatedEvent);

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