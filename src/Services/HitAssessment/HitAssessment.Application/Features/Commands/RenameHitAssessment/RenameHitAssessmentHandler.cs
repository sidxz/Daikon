using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HitAssessment.Application.Features.Commands.RenameHitAssessment
{
    public class RenameHitAssessmentHandler : IRequestHandler<RenameHitAssessmentCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<RenameHitAssessmentHandler> _logger;
        private readonly IHitAssessmentRepository _haRepository;
        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;

        public RenameHitAssessmentHandler(IMapper mapper, ILogger<RenameHitAssessmentHandler> logger, IHitAssessmentRepository haRepository, IEventSourcingHandler<HaAggregate> haEventSourcingHandler)
        {
            _mapper = mapper;
            _logger = logger;
            _haRepository = haRepository;
            _haEventSourcingHandler = haEventSourcingHandler;
        }

        public async Task<Unit> Handle(RenameHitAssessmentCommand request, CancellationToken cancellationToken)
        {
            // check if name has changed
            var existingHitAssessment = await _haRepository.ReadHaById(request.Id);
            if (existingHitAssessment == null)
            {
                _logger.LogWarning("HitAssessment not found: {Id}", request.Id);
                throw new ResourceNotFoundException(nameof(HitAssessment), request.Id);
            }

            if (existingHitAssessment.Name == request.Name)
            {
                _logger.LogWarning("HitAssessment name has not changed: {Id}", request.Id);
                return Unit.Value;
            }

            request.SetUpdateProperties(request.RequestorUserId);

            var haRenamedEvent = _mapper.Map<HaRenamedEvent>(request);

            try
            {
                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.RenameHa(haRenamedEvent);

                await _haEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HaAggregate), request.Id); ;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling UpdateHitAssessmentCommandHandler");
                throw;
            }
            return Unit.Value;
        }
    }
}