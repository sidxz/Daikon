using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Contracts.Infrastructure;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Application.DTOs.MLogixAPI;
using HitAssessment.Domain.Aggregates;
using HitAssessment.Domain.Entities;
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
        private readonly IMLogixAPIService _mLogixAPIService;

        public UpdateHaCompoundEvolutionCommandHandler(ILogger<UpdateHaCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<HaAggregate> haEventSourcingHandler,
            IHaCompoundEvolutionRepository haCompoundEvoRepository,
            IMapper mapper, IMLogixAPIService mLogixAPIService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _haCompoundEvoRepository = haCompoundEvoRepository ?? throw new ArgumentNullException(nameof(haCompoundEvoRepository));
            _haEventSourcingHandler = haEventSourcingHandler ?? throw new ArgumentNullException(nameof(haEventSourcingHandler));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));

        }

        public async Task<Unit> Handle(UpdateHaCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            // fetch existing CE
            var existingCEvo = await _haCompoundEvoRepository.ReadHaCompoundEvolutionById(request.CompoundEvolutionId);

            if (existingCEvo == null)
            {
                throw new ResourceNotFoundException(nameof(HaCompoundEvolution), request.CompoundEvolutionId);
            }

            try
            {
                var haCEUpdatedEvent = _mapper.Map<HaCompoundEvolutionUpdatedEvent>(request);

                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);

                // TODO (Future option) : check if molecule has been updated then register it
                // if (request.RequestedSMILES != existingCEvo.RequestedSMILES)
                // {
                //     if (request.RequestedSMILES is not null && request.RequestedSMILES.Value.Length > 0)
                //     {
                //         _logger.LogInformation("Will try to register molecule ...");
                //         await RegisterMoleculeAndAssignToEvent(request, haCEUpdatedEvent);
                //     }
                //     else
                //     {
                //         throw new ArgumentNullException(nameof(request.RequestedSMILES));
                //     }
                // }
                // else
                // {
                //     haCEUpdatedEvent.MoleculeId = existingCEvo.MoleculeId;
                // }

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