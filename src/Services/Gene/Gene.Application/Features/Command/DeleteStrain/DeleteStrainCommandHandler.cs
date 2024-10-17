
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Strains;
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
    private readonly IMapper _mapper;

    public DeleteStrainCommandHandler(ILogger<DeleteStrainCommandHandler> logger,
        IEventSourcingHandler<StrainAggregate> eventSourcingHandler,
        IGeneRepository geneRepository,
        IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _geneRepository = geneRepository;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteStrainCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation("DeleteStrainCommandHandler {request}", request);
      request.SetUpdateProperties(request.RequestorUserId);

      /* reject if genes exists for this strain */
      var genes = await _geneRepository.GetGenesListByStrainId(request.Id);
      if (genes.Count != 0)
      {
        throw new ResourceCannotBeDeletedException(nameof(StrainAggregate), request.Id, "Strain cannot be deleted because it has genes associated with it");
      }

      var strainDeletedEvent = _mapper.Map<StrainDeletedEvent>(request);

      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

        aggregate.DeleteGene(strainDeletedEvent);
        await _eventSourcingHandler.SaveAsync(aggregate);

        return Unit.Value;
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(StrainAggregate), request.Id);
      }


    }
  }
}