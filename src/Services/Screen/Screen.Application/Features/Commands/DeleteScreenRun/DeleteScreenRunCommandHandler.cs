using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;


namespace Screen.Application.Features.Commands.DeleteScreenRun
{
    public class DeleteScreenRunCommandHandler : IRequestHandler<DeleteScreenRunCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteScreenRunCommandHandler> _logger;
        private readonly IScreenRunRepository _screenRunRepository;

        private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;

        public DeleteScreenRunCommandHandler(ILogger<DeleteScreenRunCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler,
            IScreenRunRepository screenRunRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRunRepository = screenRunRepository ?? throw new ArgumentNullException(nameof(screenRunRepository));
            _screenEventSourcingHandler = screenEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenEventSourcingHandler));

        }

        public async Task<Unit> Handle(DeleteScreenRunCommand request, CancellationToken cancellationToken)
        {

            if (request.Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(request.Id));
            }
            request.ScreenRunId = request.Id;

            try
            {
                var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.ScreenId);
                aggregate.DeleteScreenRun(request.ScreenRunId);
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