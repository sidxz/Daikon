
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteGene
{
  public class DeleteGeneCommandHandler : IRequestHandler<DeleteGeneCommand, Unit>
  {
    private readonly ILogger<DeleteGeneCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteGeneCommandHandler(ILogger<DeleteGeneCommandHandler> logger, IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteGeneCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation($"Handling DeleteGeneCommand: {request}");
      try
      {
        request.DateModified = DateTime.UtcNow;
        var geneDeletedEvent = _mapper.Map<GeneDeletedEvent>(request);

        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        aggregate.DeleteGene(geneDeletedEvent);

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