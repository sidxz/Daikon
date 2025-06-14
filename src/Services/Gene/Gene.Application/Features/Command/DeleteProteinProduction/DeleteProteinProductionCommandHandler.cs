
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteProteinProduction
{
  public class DeleteProteinProductionCommandHandler : IRequestHandler<DeleteProteinProductionCommand, Unit>
  {
    private readonly ILogger<DeleteProteinProductionCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteProteinProductionCommandHandler(ILogger<DeleteProteinProductionCommandHandler> logger,
     IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteProteinProductionCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation("DeleteProteinProductionCommandHandler {request}", request);
      request.SetUpdateProperties(request.RequestorUserId);

      var proteinProductionDeletedEvent = _mapper.Map<GeneProteinProductionDeletedEvent>(request);
      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

        aggregate.DeleteProteinProduction(proteinProductionDeletedEvent);
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