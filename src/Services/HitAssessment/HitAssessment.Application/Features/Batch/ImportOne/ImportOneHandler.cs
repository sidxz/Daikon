using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using Daikon.Events.HitAssessment;
using Daikon.Shared.Constants.AppHitAssessment;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Application.Features.Commands.NewHaCompoundEvolution;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HitAssessment.Application.Features.Batch.ImportOne
{
    public class ImportOneHandler : IRequestHandler<ImportOneCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ImportOneHandler> _logger;
        private readonly IHitAssessmentRepository _haRepository;
        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;

        private readonly IMediator _mediator;

        public ImportOneHandler(ILogger<ImportOneHandler> logger,
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
        public async Task<Unit> Handle(ImportOneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // serialize the request to json and log
                var requestJson = JsonSerializer.Serialize(request);
                _logger.LogInformation($"Handling NewHitAssessmentCommand: {requestJson}");


                // check if name exists
                var existingHitAssessment = await _haRepository.ReadHaByName(request.Name);
                if (existingHitAssessment != null)
                {
                    _logger.LogWarning("HitAssessment name already exists: {Name}", request.Name);
                    throw new InvalidOperationException("HitAssessment name already exists");
                }

                request.IsHAComplete ??= false;
                request.IsHASuccess ??= false;
                request.IsHAPromoted ??= false;

                var newHitAssessmentCreatedEvent = _mapper.Map<HaCreatedEvent>(request);

                var aggregate = new HaAggregate(newHitAssessmentCreatedEvent);

                await _haEventSourcingHandler.SaveAsync(aggregate);

                foreach (var ceCommand in request.CompoundEvolutions)
                {
                    var addedOnStage = ceCommand.Stage;
                    ceCommand.CompoundEvolutionId = Guid.NewGuid();
                    ceCommand.ImportMode = true;
                    
                    if (addedOnStage != "HA")
                        continue;

                    try
                    {
                        await _mediator.Send(ceCommand, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while creating initial HaCompoundEvolution");
                        throw;
                    }
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