using AutoMapper;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;

namespace Screen.Application.Features.Commands.NewScreenRun
{
    public class NewScreenRunCommandHandler : IRequestHandler<NewScreenRunCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewScreenRunCommandHandler> _logger;
        private readonly IScreenRunRepository _screenRunRepository;

        private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;

        public NewScreenRunCommandHandler(ILogger<NewScreenRunCommandHandler> logger, 
            IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler,
            IScreenRunRepository screenRunRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRunRepository = screenRunRepository ?? throw new ArgumentNullException(nameof(screenRunRepository));
            _screenEventSourcingHandler = screenEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenEventSourcingHandler));
          
        }

        public async Task<Unit> Handle(NewScreenRunCommand request, CancellationToken cancellationToken)
        {
           
           var newScreenRun = _mapper.Map<Domain.Entities.ScreenRun>(request);
            
            
            return Unit.Value;
        }

    }
    
}