
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Project;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.Domain.Aggregates;

namespace Project.Application.Features.Commands.RenameProject
{
    public class RenameProjectHandler : IRequestHandler<RenameProjectCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<RenameProjectHandler> _logger;
        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;

        public RenameProjectHandler(ILogger<RenameProjectHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
        }
        public async Task<Unit> Handle(RenameProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("RenameProjectCommand: {Id}", request.Id);

            // Experimental: (Code Improvement) Removing dependency on the repository
            // Name check is done in the aggregate

            request.SetUpdateProperties(request.RequestorUserId);

            try
            {
                var projectRenamedEvent = _mapper.Map<ProjectRenamedEvent>(request);
                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);
                aggregate.RenameProject(projectRenamedEvent);
                await _projectEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping ProjectRenamedEvent");
                throw;
            }

            return Unit.Value;

        }
    }
}