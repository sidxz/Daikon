
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteResistanceMutation
{
  public class DeleteResistanceMutationCommandHandler : IRequestHandler<DeleteResistanceMutationCommand, Unit>
  {
    private readonly ILogger<DeleteResistanceMutationCommandHandler> _logger;

    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

    public DeleteResistanceMutationCommandHandler(ILogger<DeleteResistanceMutationCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(DeleteResistanceMutationCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var resistanceMutation = new Domain.Entities.ResistanceMutation
        {
          Id = request.Id,
          GeneId = request.GeneId,
          ResistanceMutationId = request.ResistanceMutationId,
          Mutation = ""
        };

        aggregate.DeleteResistanceMutation(resistanceMutation);
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