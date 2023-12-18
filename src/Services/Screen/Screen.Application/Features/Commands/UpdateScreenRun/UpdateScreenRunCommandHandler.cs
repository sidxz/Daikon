using AutoMapper;
using CQRS.Core.Handlers;
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

        private readonly IEventSourcingHandler<ScreenAggregate> _screenRunEventSourcingHandler;
    
        public UpdateScreenRunCommandHandler(ILogger<UpdateScreenRunCommandHandler> logger,
            IEventSourcingHandler<ScreenAggregate> screenRunEventSourcingHandler,
            IScreenRunRepository screenRunRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenRunRepository = screenRunRepository ?? throw new ArgumentNullException(nameof(screenRunRepository));
            _screenRunEventSourcingHandler = screenRunEventSourcingHandler ?? throw new ArgumentNullException(nameof(screenRunEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateScreenRunCommand request, CancellationToken cancellationToken)
        {
        
            var modScreenRun = _mapper.Map<Domain.Entities.ScreenRun>(request);
            

            return Unit.Value;
        }
    
    
    
    
    
    }


        
    
}