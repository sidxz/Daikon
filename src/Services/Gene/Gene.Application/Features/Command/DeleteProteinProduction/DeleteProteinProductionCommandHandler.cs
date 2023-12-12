
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteProteinProduction
{
  public class DeleteProteinProductionCommandHandler : IRequestHandler<DeleteProteinProductionCommand, Unit>
  {
    private readonly ILogger<DeleteProteinProductionCommandHandler> _logger;

    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

    public DeleteProteinProductionCommandHandler(ILogger<DeleteProteinProductionCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(DeleteProteinProductionCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var proteinProduction = new Domain.Entities.ProteinProduction
        {
          Id = request.Id,
          GeneId = request.GeneId,
          ProteinProductionId = request.ProteinProductionId,
          Production = ""
        };

        aggregate.DeleteProteinProduction(proteinProduction);
        await _eventSourcingHandler.SaveAsync(aggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(GeneAggregate), request.Id);
      }
      return Unit.Value;

    }
  }
}