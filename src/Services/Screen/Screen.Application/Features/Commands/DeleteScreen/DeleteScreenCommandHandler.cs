
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Screen.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Daikon.Events.Screens;

namespace Screen.Application.Features.Commands.DeleteScreen
{
  public class DeleteScreenCommandHandler : IRequestHandler<DeleteScreenCommand, Unit>
  {
    private readonly ILogger<DeleteScreenCommandHandler> _logger;
    private readonly IEventSourcingHandler<ScreenAggregate> _screenEventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteScreenCommandHandler(ILogger<DeleteScreenCommandHandler> logger,
    IEventSourcingHandler<ScreenAggregate> screenEventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _screenEventSourcingHandler = screenEventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteScreenCommand request, CancellationToken cancellationToken)
    {


      try
      {
        request.SetUpdateProperties(request.RequestorUserId);
        
        var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.Id);

        var screenDeletedEvent = _mapper.Map<ScreenDeletedEvent>(request);

        aggregate.DeleteScreen(screenDeletedEvent);

        await _screenEventSourcingHandler.SaveAsync(aggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(ScreenAggregate), request.Id);
      }
      return Unit.Value;

    }
  }
}