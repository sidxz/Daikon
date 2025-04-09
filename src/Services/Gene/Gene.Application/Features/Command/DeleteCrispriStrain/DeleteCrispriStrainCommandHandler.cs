
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteCrispriStrain
{
  public class DeleteCrispriStrainCommandHandler : IRequestHandler<DeleteCrispriStrainCommand, Unit>
  {
    private readonly ILogger<DeleteCrispriStrainCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteCrispriStrainCommandHandler(ILogger<DeleteCrispriStrainCommandHandler> logger,
    IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
    IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteCrispriStrainCommand request, CancellationToken cancellationToken)
    {

      _logger.LogInformation("DeleteCrispriStrainCommandHandler {request}", request);
      request.SetUpdateProperties(request.RequestorUserId);

      var geneCrispriStrainDeletedEvent = _mapper.Map<GeneCrispriStrainDeletedEvent>(request);

      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

        aggregate.DeleteCrispriStrain(geneCrispriStrainDeletedEvent);
        await _eventSourcingHandler.SaveAsync(aggregate);
        
        return Unit.Value;
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(GeneAggregate), request.Id);
      }

    }
  }
}