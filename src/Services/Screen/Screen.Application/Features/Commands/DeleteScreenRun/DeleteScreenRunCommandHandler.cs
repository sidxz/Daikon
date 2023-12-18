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

        private readonly IEventSourcingHandler<ScreenAggregate> _screenRunEventSourcingHandler;

        public DeleteScreenRunCommandHandler(ILogger<DeleteScreenRunCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenRunEventSourcingHandler,
            IScreenRunRepository screenRunRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRunRepository = screenRunRepository ?? throw new ArgumentNullException(nameof(screenRunRepository));
            _screenRunEventSourcingHandler = screenRunEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenRunEventSourcingHandler));

        }

        public async Task<Unit> Handle(DeleteScreenRunCommand request, CancellationToken cancellationToken)
        {

            if (request.Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(request.Id));
            }

            request.ScreenRunId = request.Id;

            
            return Unit.Value;
        }
    }
}