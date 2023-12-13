
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Target.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Target.Application.Features.Command.DeleteTarget
{
  public class DeleteTargetCommandHandler : IRequestHandler<DeleteTargetCommand, Unit>
  {
    private readonly ILogger<DeleteTargetCommandHandler> _logger;
    private readonly IEventSourcingHandler<TargetAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteTargetCommandHandler(ILogger<DeleteTargetCommandHandler> logger, IEventSourcingHandler<TargetAggregate> eventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteTargetCommand request, CancellationToken cancellationToken)
    {


      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        var gene = new Domain.Entities.Target
        {
          Id = request.Id,
          Name = "",
        };

        aggregate.DeleteTarget(gene, _mapper);
        await _eventSourcingHandler.SaveAsync(aggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(TargetAggregate), request.Id);
      }
      return Unit.Value;

    }
  }
}