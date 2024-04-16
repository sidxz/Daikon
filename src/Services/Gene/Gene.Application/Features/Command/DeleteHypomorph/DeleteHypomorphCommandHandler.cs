
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteHypomorph
{
  public class DeleteHypomorphCommandHandler : IRequestHandler<DeleteHypomorphCommand, Unit>
  {
    private readonly ILogger<DeleteHypomorphCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteHypomorphCommandHandler(ILogger<DeleteHypomorphCommandHandler> logger,
      IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
      IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteHypomorphCommand request, CancellationToken cancellationToken)
    {

      _logger.LogInformation("DeleteHypomorphCommandHandler {request}", request);
      var geneHypomorphDeletedEvent = _mapper.Map<GeneHypomorphDeletedEvent>(request);
      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

        aggregate.DeleteHypomorph(geneHypomorphDeletedEvent);
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