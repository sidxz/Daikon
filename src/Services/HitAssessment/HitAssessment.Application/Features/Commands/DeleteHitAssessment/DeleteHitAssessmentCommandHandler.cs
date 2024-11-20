
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using HitAssessment.Domain.Aggregates;
using Daikon.Events.HitAssessment;

namespace HitAssessment.Application.Features.Commands.DeleteHitAssessment
{
  public class DeleteHitAssessmentCommandHandler : IRequestHandler<DeleteHitAssessmentCommand, Unit>
  {
    private readonly ILogger<DeleteHitAssessmentCommandHandler> _logger;
    private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteHitAssessmentCommandHandler(ILogger<DeleteHitAssessmentCommandHandler> logger,
    IEventSourcingHandler<HaAggregate> haEventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _haEventSourcingHandler = haEventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteHitAssessmentCommand request, CancellationToken cancellationToken)
    {


      try
      {
        request.SetUpdateProperties(request.RequestorUserId);
        
        var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);

        var haDeletedEvent = _mapper.Map<HaDeletedEvent>(request);

        aggregate.DeleteHa(haDeletedEvent);

        await _haEventSourcingHandler.SaveAsync(aggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(HaAggregate), request.Id);
      }
      return Unit.Value;

    }
  }
}