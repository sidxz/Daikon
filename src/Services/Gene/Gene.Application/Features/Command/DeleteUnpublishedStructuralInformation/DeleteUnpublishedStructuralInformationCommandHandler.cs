
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteUnpublishedStructuralInformation
{
  public class DeleteUnpublishedStructuralInformationCommandHandler : IRequestHandler<DeleteUnpublishedStructuralInformationCommand, Unit>
  {
    private readonly ILogger<DeleteUnpublishedStructuralInformationCommandHandler> _logger;

    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

    public DeleteUnpublishedStructuralInformationCommandHandler(ILogger<DeleteUnpublishedStructuralInformationCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(DeleteUnpublishedStructuralInformationCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var unpublishedStructuralInformation = new Domain.Entities.UnpublishedStructuralInformation
        {
          Id = request.Id,
          GeneId = request.GeneId,
          UnpublishedStructuralInformationId = request.UnpublishedStructuralInformationId,
          Organization = ""
        };

        aggregate.DeleteUnpublishedStructuralInformation(unpublishedStructuralInformation);
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