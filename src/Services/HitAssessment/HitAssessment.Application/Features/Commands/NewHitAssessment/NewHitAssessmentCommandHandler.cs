using System.Text.Json;
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.EventStore.Handlers;
using Daikon.Events.HitAssessment;
using Daikon.Shared.Constants.AppHitAssessment;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Application.Features.Commands.NewHaCompoundEvolution;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HitAssessment.Application.Features.Commands.NewHitAssessment
{
    public class NewHitAssessmentCommandHandler : IRequestHandler<NewHitAssessmentCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewHitAssessmentCommandHandler> _logger;
        private readonly IHitAssessmentRepository _haRepository;
        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;

        private readonly IMediator _mediator;

        public NewHitAssessmentCommandHandler(ILogger<NewHitAssessmentCommandHandler> logger,
            IEventSourcingHandler<HaAggregate> haEventSourcingHandler,
            IHitAssessmentRepository haRepository,
            IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _haRepository = haRepository ?? throw new ArgumentNullException(nameof(haRepository));
            _haEventSourcingHandler = haEventSourcingHandler ?? throw new ArgumentNullException(nameof(haEventSourcingHandler));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(NewHitAssessmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // handle dates
                var now = DateTime.UtcNow;
                request.SetCreateProperties(request.RequestorUserId);

                // check if name exists
                var existingHitAssessment = await _haRepository.ReadHaByName(request.Name);
                if (existingHitAssessment != null)
                {
                    _logger.LogWarning("HitAssessment name already exists: {Name}", request.Name);
                    throw new InvalidOperationException("HitAssessment name already exists");
                }



                request.StatusLastModifiedDate = now;
                // set HAPredictedStartDate to 10 days now if not set
                request.HaPredictedStartDate ??= new DVariable<DateTime>(now.AddDays(10));


                request.Status ??= new DVariable<string>(nameof(HitAssessmentStatus.ReadyForHA));
                request.IsHAComplete ??= false;
                request.IsHASuccess ??= false;
                request.IsHAPromoted ??= false;

                var newHitAssessmentCreatedEvent = _mapper.Map<HaCreatedEvent>(request);

                var aggregate = new HaAggregate(newHitAssessmentCreatedEvent);

                await _haEventSourcingHandler.SaveAsync(aggregate);

                // Crete a compound evolution for the hit assessment as a starting point
                // Create compound evolution only if compoundId is provided
                if (request.CompoundId != Guid.Empty)
                {
                    var newHaCompoundEvolutionCommand = new NewHaCompoundEvolutionCommand
                    {
                        Id = request.Id,
                        CompoundEvolutionId = Guid.NewGuid(),
                        MoleculeId = request.CompoundId,
                        EvolutionDate = request.DateCreated,
                        Stage = "HA",
                        Notes = new DVariable<string>("Initial HA Compound"),
                        MIC = request.CompoundMIC ?? "0",
                        IC50 = request.CompoundIC50 ?? "0",
                        RequestedSMILES = request.CompoundSMILES,
                        ImportMode = false
                    };

                    try
                    {
                        await _mediator.Send(newHaCompoundEvolutionCommand, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while creating initial HaCompoundEvolution");
                        throw;
                    }
                }

                else
                {
                    _logger.LogWarning("Creating Unlinked HA. No compoundId provided for HitAssessment: {HitAssessmentId}", request.Id);
                }
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling NewHitAssessmentCommand");
                throw;
            }
        }
    }
}
