using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Infrastructure;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Project.Application.Features.Commands.NewProjectCompoundEvolution
{
    public class NewProjectCompoundEvolutionCommandHandler : IRequestHandler<NewProjectCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewProjectCompoundEvolutionCommandHandler> _logger;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvoRepository;

        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;
        private readonly IMolDbAPIService _molDbAPIService;

        public NewProjectCompoundEvolutionCommandHandler(ILogger<NewProjectCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectCompoundEvolutionRepository projectCompoundEvoRepository,
            IMapper mapper, IMolDbAPIService molDbAPIService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectCompoundEvoRepository = projectCompoundEvoRepository ?? throw new ArgumentNullException(nameof(projectCompoundEvoRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
            _molDbAPIService = molDbAPIService ?? throw new ArgumentNullException(nameof(molDbAPIService));

        }

        public async Task<Unit> Handle(NewProjectCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var projectCEAddedEvent = _mapper.Map<ProjectCompoundEvolutionAddedEvent>(request);

                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);
                Guid compoundId;
                try
                {
                    compoundId = await _molDbAPIService.RegisterCompound("Test", request.CompoundStructureSMILES);
                    projectCEAddedEvent.CompoundId = compoundId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while calling MolDbAPI");
                    _logger.LogError(ex.Message);
                    throw new Exception(nameof(ProjectAggregate));
                }


                aggregate.AddCompoundEvolution(projectCEAddedEvent);

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