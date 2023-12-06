
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Gene.Application.Contracts.Persistence;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteStrain
{
  public class DeleteStrainCommandHandler : IRequestHandler<DeleteStrainCommand, Unit>
  {
    private readonly ILogger<DeleteStrainCommandHandler> _logger;

    private readonly IEventSourcingHandler<StrainAggregate> _eventSourcingHandler;

    private readonly IGeneRepository _geneRepository;

    public DeleteStrainCommandHandler(ILogger<DeleteStrainCommandHandler> logger, IEventSourcingHandler<StrainAggregate> eventSourcingHandler, IGeneRepository geneRepository)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _geneRepository = geneRepository;
    }

    public async Task<Unit> Handle(DeleteStrainCommand request, CancellationToken cancellationToken)
    {

      /* reject if genes exists for this strain */
      var genes = await _geneRepository.GetGenesListByStrainId(request.Id);
      if (genes.Any())
      {
        throw new ResourceCannotBeDeletedException(nameof(StrainAggregate), request.Id, "Strain cannot be deleted because it has genes associated with it");
      }

      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var strain = new Domain.Entities.Strain
        {
          Id = request.Id,
        };

        aggregate.DeleteGene(strain);
        await _eventSourcingHandler.SaveAsync(aggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
      }
      return Unit.Value;

    }
  }
}