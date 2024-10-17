
using AutoMapper;
using CQRS.Core.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Target.Domain.Aggregates;
using CQRS.Core.Comparators;
using CQRS.Core.Exceptions;
using Daikon.Events.Targets;
namespace Target.Application.Features.Commands.UpdateTPQ
{
    public class UpdateTPQHandler : IRequestHandler<UpdateTPQCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<UpdateTPQHandler> _logger;
        private readonly IEventSourcingHandler<TPQuestionnaireAggregate> _questionnaireESH;

        private readonly IPQResponseRepository _pqResponseRepository;

        public UpdateTPQHandler(IMapper mapper, ILogger<UpdateTPQHandler> logger, IEventSourcingHandler<TPQuestionnaireAggregate> questionnaireESH, IPQResponseRepository pqResponseRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _questionnaireESH = questionnaireESH;
            _pqResponseRepository = pqResponseRepository;
        }


        public async Task<Unit> Handle(UpdateTPQCommand request, CancellationToken cancellationToken)
        {
            request.SetUpdateProperties(request.RequestorUserId);
            _logger.LogInformation($"Handling UpdateTPQCommand:");
            // fetch the existing TPQ.
            var existingTPQ = await _pqResponseRepository.ReadById(request.Id);
            var tpqUpdatedEvent = _mapper.Map<TargetPromotionQuestionnaireUpdatedEvent>(request);


            // check if the TPQ is Verified, if it is, dont allow changes to RequestedTargetName, RequestedAssociatedGenes
            if (existingTPQ.IsVerified == true)
            {
                tpqUpdatedEvent = _mapper.Map<TargetPromotionQuestionnaireUpdatedEvent>(existingTPQ);
                tpqUpdatedEvent.Response = request.Response;

            }
            else
            {
                tpqUpdatedEvent.IsVerified = false;
            }

            try
            {
                var aggregate = await _questionnaireESH.GetByAsyncId(request.Id);

                aggregate.UpdatePQResponse(tpqUpdatedEvent);

                await _questionnaireESH.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogError(ex, "Aggregate not found");
            }

            return Unit.Value;

        }
    }
}