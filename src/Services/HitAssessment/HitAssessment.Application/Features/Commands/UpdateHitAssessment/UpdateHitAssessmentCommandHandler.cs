using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Application.Features.Commands.UpdateHitAssessment;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HitAssessment.Application.Features.Commands.NewHitAssessment
{
    public class UpdateHitAssessmentCommandHandler : IRequestHandler<UpdateHitAssessmentCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHitAssessmentCommandHandler> _logger;
        private readonly IHitAssessmentRepository _haRepository;
        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;

        public UpdateHitAssessmentCommandHandler(ILogger<UpdateHitAssessmentCommandHandler> logger,
            IEventSourcingHandler<HaAggregate> haEventSourcingHandler,
            IHitAssessmentRepository haRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _haRepository = haRepository ?? throw new ArgumentNullException(nameof(haRepository));
            _haEventSourcingHandler = haEventSourcingHandler ?? throw new ArgumentNullException(nameof(haEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateHitAssessmentCommand request, CancellationToken cancellationToken)
        {
            var haUpdatedEvent = _mapper.Map<HaUpdatedEvent>(request);

            try
            {
                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateHa(haUpdatedEvent);

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
