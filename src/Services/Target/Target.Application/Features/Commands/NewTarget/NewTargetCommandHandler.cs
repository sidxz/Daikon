
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Target.Application.Features.Command.NewTarget
{
    public class NewTargetCommandHandler : IRequestHandler<NewTargetCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewTargetCommandHandler> _logger;

        private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
        //private readonly ITargetRepository _targetRepository;

        public NewTargetCommandHandler(ILogger<NewTargetCommandHandler> logger, 
            IEventSourcingHandler<TargetAggregate> eventSourcingHandler, 
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventSourcingHandler = eventSourcingHandler ?? throw new ArgumentNullException(nameof(eventSourcingHandler));
          
        }


        public async Task<Unit> Handle(NewTargetCommand request, CancellationToken cancellationToken)
        {
           
            // check if target (targetName) already exists; reject if it does

            // var targetExists = await _targetRepository.ReadTargetByAccession(request.Name);
            // if (targetExists != null)
            // {
            //     throw new DuplicateEntityRequestException(nameof(NewTargetCommand), request.Name);
            // }

            

            var target = _mapper.Map<Domain.Entities.Target>(request);


            var aggregate = new TargetAggregate(target, _mapper);
            await _eventSourcingHandler.SaveAsync(aggregate);

            return Unit.Value;

        }
    }

}