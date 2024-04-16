
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteEssentiality
{
  public class DeleteEssentialityCommandHandler : IRequestHandler<DeleteEssentialityCommand, Unit>
  {
    private readonly ILogger<DeleteEssentialityCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteEssentialityCommandHandler(ILogger<DeleteEssentialityCommandHandler> logger,
     IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
     IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteEssentialityCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation("DeleteEssentialityCommandHandler {request}", request);

      var geneEssentialityDeletedEvent = _mapper.Map<GeneEssentialityDeletedEvent>(request);
      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

        aggregate.DeleteEssentiality(geneEssentialityDeletedEvent);
        await _eventSourcingHandler.SaveAsync(aggregate);

        return Unit.Value;

      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(GeneAggregate), request.Id);
      }
      
    }
  }
}