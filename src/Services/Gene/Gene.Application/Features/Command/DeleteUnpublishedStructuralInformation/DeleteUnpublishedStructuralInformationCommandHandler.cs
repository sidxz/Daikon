
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Gene;
using Gene.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Features.Command.DeleteUnpublishedStructuralInformation
{
  public class DeleteUnpublishedStructuralInformationCommandHandler : IRequestHandler<DeleteUnpublishedStructuralInformationCommand, Unit>
  {
    private readonly ILogger<DeleteUnpublishedStructuralInformationCommandHandler> _logger;
    private readonly IEventSourcingHandler<GeneAggregate> _eventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteUnpublishedStructuralInformationCommandHandler(ILogger<DeleteUnpublishedStructuralInformationCommandHandler> logger,
     IEventSourcingHandler<GeneAggregate> eventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _eventSourcingHandler = eventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteUnpublishedStructuralInformationCommand request, CancellationToken cancellationToken)
    {
      _logger.LogInformation("DeleteUnpublishedStructuralInformationCommandHandler {request}", request);
      request.SetUpdateProperties(request.RequestorUserId);

      var unpublishedStructuralInformationDeletedEvent = _mapper.Map<GeneUnpublishedStructuralInformationDeletedEvent>(request);

      try
      {
        var aggregate = await _eventSourcingHandler.GetByAsyncId(request.Id);
        
        aggregate.DeleteUnpublishedStructuralInformation(unpublishedStructuralInformationDeletedEvent);
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