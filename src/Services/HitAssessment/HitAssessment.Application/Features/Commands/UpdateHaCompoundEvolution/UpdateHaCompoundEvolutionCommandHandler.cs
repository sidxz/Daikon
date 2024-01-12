using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Contracts.Infrastructure;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution
{
    public class UpdateHaCompoundEvolutionCommandHandler : IRequestHandler<UpdateHaCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHaCompoundEvolutionCommandHandler> _logger;
        private readonly IHaCompoundEvolutionRepository _haCompoundEvoRepository;

        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;
        private readonly IMolDbAPIService _molDbAPIService;

        public UpdateHaCompoundEvolutionCommandHandler(ILogger<UpdateHaCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<HaAggregate> haEventSourcingHandler,
            IHaCompoundEvolutionRepository haCompoundEvoRepository,
            IMapper mapper, IMolDbAPIService molDbAPIService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _haCompoundEvoRepository = haCompoundEvoRepository ?? throw new ArgumentNullException(nameof(haCompoundEvoRepository));
            _haEventSourcingHandler = haEventSourcingHandler ?? throw new ArgumentNullException(nameof(haEventSourcingHandler));
            _molDbAPIService = molDbAPIService ?? throw new ArgumentNullException(nameof(molDbAPIService));

        }

        public async Task<Unit> Handle(UpdateHaCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var haCEUpdatedEvent = _mapper.Map<HaCompoundEvolutionUpdatedEvent>(request);

                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);
                Guid compoundId;
                try
                {
                    compoundId = await _molDbAPIService.RegisterCompound("Test", request.CompoundStructureSMILES);
                    haCEUpdatedEvent.CompoundId = compoundId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while calling MolDbAPI");
                    _logger.LogError(ex.Message);
                    throw new Exception(nameof(HaAggregate));
                }


                aggregate.UpdateCompoundEvolution(haCEUpdatedEvent);

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