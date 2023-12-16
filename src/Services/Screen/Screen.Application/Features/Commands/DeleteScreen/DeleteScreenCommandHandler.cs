
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Screen.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;

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
        var aggregate = await _screenEventSourcingHandler.GetByAsyncId(request.Id);

        var screenToBeDeleted = new Domain.Entities.Screen
        {
          Id = request.Id,
          Name = "",
        };

        aggregate.DeleteScreen(screenToBeDeleted);
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