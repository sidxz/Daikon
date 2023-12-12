
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteProteinActivityAssay
{
  public class DeleteProteinActivityAssayCommandHandler : IRequestHandler<DeleteProteinActivityAssayCommand, Unit>
  {
    private readonly ILogger<DeleteProteinActivityAssayCommandHandler> _logger;

    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;

    public DeleteProteinActivityAssayCommandHandler(ILogger<DeleteProteinActivityAssayCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(DeleteProteinActivityAssayCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var proteinActivityAssay = new Domain.Entities.ProteinActivityAssay
        {
          Id = request.Id,
          GeneId = request.GeneId,
          ProteinActivityAssayId = request.ProteinActivityAssayId,
          Assay = ""
        };

        aggregate.DeleteProteinActivityAssay(proteinActivityAssay);
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