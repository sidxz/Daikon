
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteCrispriStrain
{
  public class DeleteCrispriStrainCommandHandler : IRequestHandler<DeleteCrispriStrainCommand, Unit>
  {
    private readonly ILogger<DeleteCrispriStrainCommandHandler> _logger;

    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

    public DeleteCrispriStrainCommandHandler(ILogger<DeleteCrispriStrainCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(DeleteCrispriStrainCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var crispriStrain = new Domain.Entities.CrispriStrain
        {
          Id = request.Id,
          GeneId = request.GeneId,
          CrispriStrainId = request.CrispriStrainId,
          CrispriStrainName = ""
        };

        aggregate.DeleteCrispriStrain(crispriStrain);
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