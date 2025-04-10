
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteResistanceMutation
{
  public class DeleteResistanceMutationCommandHandler : IRequestHandler<DeleteResistanceMutationCommand, Unit>
  {
    private readonly ILogger<DeleteResistanceMutationCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteResistanceMutationCommandHandler(ILogger<DeleteResistanceMutationCommandHandler> logger, 
      IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteResistanceMutationCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation("DeleteResistanceMutationCommandHandler {request}", request);
      request.SetUpdateProperties(request.RequestorUserId);

      var resistanceMutationDeletedEvent = _mapper.Map<GeneResistanceMutationDeletedEvent>(request);

      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
      
        aggregate.DeleteResistanceMutation(resistanceMutationDeletedEvent);
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