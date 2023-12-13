
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteEssentiality
{
  public class DeleteEssentialityCommandHandler : IRequestHandler<DeleteEssentialityCommand, Unit>
  {
    private readonly ILogger<DeleteEssentialityCommandHandler> _logger;

    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

    public DeleteEssentialityCommandHandler(ILogger<DeleteEssentialityCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(DeleteEssentialityCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var essentiality = new Domain.Entities.Essentiality
        {
          Id = request.Id,
          GeneId = request.GeneId,
          EssentialityId = request.EssentialityId,
          Classification = ""
        };

        aggregate.DeleteEssentiality(essentiality);
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