
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteHypomorph
{
  public class DeleteHypomorphCommandHandler : IRequestHandler<DeleteHypomorphCommand, Unit>
  {
    private readonly ILogger<DeleteHypomorphCommandHandler> _logger;

    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

    public DeleteHypomorphCommandHandler(ILogger<DeleteHypomorphCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(DeleteHypomorphCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var hypomorph = new Domain.Entities.Hypomorph
        {
          Id = request.Id,
          GeneId = request.GeneId,
          HypomorphId = request.HypomorphId,
          KnockdownStrain = ""
        };

        aggregate.DeleteHypomorph(hypomorph);
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