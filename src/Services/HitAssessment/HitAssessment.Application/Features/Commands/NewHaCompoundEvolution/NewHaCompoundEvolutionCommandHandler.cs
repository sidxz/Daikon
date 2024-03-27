using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Contracts.Infrastructure;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HitAssessment.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewHaCompoundEvolutionCommandHandler : IRequestHandler<NewHaCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewHaCompoundEvolutionCommandHandler> _logger;
        private readonly IHaCompoundEvolutionRepository _haCompoundEvoRepository;

        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;
        private readonly IMolDbAPIService _molDbAPIService;

        public NewHaCompoundEvolutionCommandHandler(ILogger<NewHaCompoundEvolutionCommandHandler> logger,
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

        public async Task<Unit> Handle(NewHaCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var haCEAddedEvent = _mapper.Map<HaCompoundEvolutionAddedEvent>(request);

                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);
                Guid compoundId;

                if (request.CompoundId == null || request.CompoundId == Guid.Empty)
                {
                    try
                    {
                        _logger.LogInformation("Registering compound with MolDbAPI");
                        compoundId = await _molDbAPIService.RegisterCompound(request.CompoundName ?? "Unknown", request.CompoundStructureSMILES);
                        haCEAddedEvent.CompoundId = compoundId;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while calling MolDbAPI");
                        _logger.LogError(ex.Message);
                        throw new Exception(nameof(HaAggregate));
                    }
                }
                else
                {
                    _logger.LogInformation("CompoundId provided in request");
                    compoundId = request.CompoundId.Value;
                }

                aggregate.AddCompoundEvolution(haCEAddedEvent);

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