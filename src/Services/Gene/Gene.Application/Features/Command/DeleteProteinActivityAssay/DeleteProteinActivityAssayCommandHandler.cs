
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteProteinActivityAssay
{
  public class DeleteProteinActivityAssayCommandHandler : IRequestHandler<DeleteProteinActivityAssayCommand, Unit>
  {
    private readonly ILogger<DeleteProteinActivityAssayCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteProteinActivityAssayCommandHandler(ILogger<DeleteProteinActivityAssayCommandHandler> logger,
     IEventSourcingHandler<GeneAggregate> eventSourcingHandler,
     IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteProteinActivityAssayCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation("DeleteProteinActivityAssayCommandHandler {request}", request);
      request.SetUpdateProperties(request.RequestorUserId);

      var proteinActivityAssayDeletedEvent = _mapper.Map<GeneProteinActivityAssayDeletedEvent>(request);
      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);

        aggregate.DeleteProteinActivityAssay(proteinActivityAssayDeletedEvent);
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