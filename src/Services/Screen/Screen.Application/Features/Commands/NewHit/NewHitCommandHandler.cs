using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;
using Screen.Domain.Entities;


namespace Screen.Application.Features.Commands.NewHit
{
  public class NewHitCommandHandler : IRequestHandler<NewHitCommand, Unit>
  {

    private readonly IMapper _mapper;
    private readonly ILogger<NewHitCommandHandler> _logger;
    private readonly IHitRepository _hitRepository;

    private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;


    public NewHitCommandHandler(ILogger<NewHitCommandHandler> logger,
        IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
        IHitRepository hitRepository,
        IMapper mapper)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _hitRepository = hitRepository ?? throw new ArgumentNullException(nameof(hitRepository));
      _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));

    }

    public async Task<Unit> Handle(NewHitCommand request, CancellationToken cancellationToken)
    {

      if (request.Id == Guid.Empty)
      {
        throw new ArgumentNullException(nameof(request.Id));
      }

      var newHit = _mapper.Map<Hit>(request);
      newHit.HitCollectionId = request.Id;

      try
      {
        var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.HitCollectionId);
        aggregate.AddHit(newHit);
        await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(HitCollectionAggregate), request.Id);
      }
      return Unit.Value;
    }
  }
}