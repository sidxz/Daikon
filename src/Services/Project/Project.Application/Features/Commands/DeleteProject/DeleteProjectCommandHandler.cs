
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Project.Domain.Aggregates;
using Daikon.Events.Project;

namespace Project.Application.Features.Commands.DeleteProject
{
  public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
  {
    private readonly ILogger<DeleteProjectCommandHandler> _logger;
    private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;
    private readonly IMapper _mapper;

    public DeleteProjectCommandHandler(ILogger<DeleteProjectCommandHandler> logger,
    IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler, IMapper mapper)
    {
      _logger = logger;
      _projectEventSourcingHandler = projectEventSourcingHandler;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {

      try
      {
        request.SetUpdateProperties(request.RequestorUserId);
        
        var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);

        var projectDeletedEvent = _mapper.Map<ProjectDeletedEvent>(request);

        aggregate.DeleteProject(projectDeletedEvent);

        await _projectEventSourcingHandler.SaveAsync(aggregate);
      }
      catch (AggregateNotFoundException ex)
      {
        _logger.LogWarning(ex, "Aggregate not found");
        throw new ResourceNotFoundException(nameof(ProjectAggregate), request.Id);
      }
      return Unit.Value;

    }
  }
}