using AutoMapper;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;

namespace Screen.Application.Features.Commands.NewHitCollection
{
    public class NewHitCollectionCommandHandler : IRequestHandler<NewHitCollectionCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewHitCollectionCommandHandler> _logger;
        private readonly IHitCollectionRepository _hitCollectionRepository;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;
        
        public NewHitCollectionCommandHandler(ILogger<NewHitCollectionCommandHandler> logger, 
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitCollectionRepository hitCollectionRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitCollectionRepository = hitCollectionRepository ?? throw new ArgumentNullException(nameof(hitCollectionRepository));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
          
        }

        public async Task<Unit> Handle(NewHitCollectionCommand request, CancellationToken cancellationToken)
        {
           
            // check if name exists
            var existingHitCollection = await _hitCollectionRepository.ReadHitCollectionByName(request.Name);
            if (existingHitCollection != null)
            {
                throw new InvalidOperationException("HitCollection name already exists");
            }
            

            var newHitCollection = _mapper.Map<Domain.Entities.HitCollection>(request);
            var aggregate = new HitCollectionAggregate(newHitCollection, _mapper);
            await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);

            return Unit.Value;

        }
    }
}