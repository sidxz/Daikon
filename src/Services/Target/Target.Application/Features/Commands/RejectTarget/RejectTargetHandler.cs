using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Events.Targets;
using Daikon.EventStore.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Domain.Aggregates;

namespace Target.Application.Features.Commands.RejectTarget
{
    public class RejectTargetHandler : IRequestHandler<RejectTargetCommand, Unit>
    {
        private readonly ILogger<RejectTargetHandler> _logger;
        private readonly IEventSourcingHandler<TPQuestionnaireAggregate> _questionnaireESH;
        private readonly IMapper _mapper;

        public RejectTargetHandler(ILogger<RejectTargetHandler> logger, IEventSourcingHandler<TPQuestionnaireAggregate> questionnaireESH, IMapper mapper)
        {
            _logger = logger;
            _questionnaireESH = questionnaireESH;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(RejectTargetCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            _logger.LogInformation("Handling rejection for target: {TargetId}", request.Id);
            try
            {
                var aggregate = await _questionnaireESH.GetByAsyncId(request.Id);

                var targetRejectedEvent = _mapper.Map<TargetPromotionQuestionnaireDeletedEvent>(request);

                aggregate.DeletePQResponse(targetRejectedEvent);
                _logger.LogInformation("Target rejected: {TargetId}", request.Id);

                await _questionnaireESH.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(TargetAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}