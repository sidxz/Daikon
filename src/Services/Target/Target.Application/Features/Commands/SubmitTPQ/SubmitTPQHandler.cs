
using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.Targets;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Target.Domain.Aggregates;

namespace Target.Application.Features.Commands.SubmitTPQ
{
    public class SubmitTPQHandler : IRequestHandler<SubmitTPQCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<SubmitTPQHandler> _logger;
        private readonly IEventSourcingHandler<TPQuestionnaireAggregate> _questionnaireESH;

        public SubmitTPQHandler(IMapper mapper, ILogger<SubmitTPQHandler> logger, IEventSourcingHandler<TPQuestionnaireAggregate> questionnaireESH)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _questionnaireESH = questionnaireESH ?? throw new ArgumentNullException(nameof(questionnaireESH));
        }

        public async Task<Unit> Handle(SubmitTPQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Handling SubmitTPQCommand: {request.ToJson()}");

                /*public List<(string QIdentification, string Answer, string Description)> Response { get; set; }
                public List<Tuple<string, string, string>> Response { get; set; }
                capitalize QIdentification and Answer
                */
                if (request.Response != null) // Check if the response is not null
                {
                    request.Response = request.Response.Select(tuple =>
                        new Tuple<string, string, string>(
                            tuple.Item1?.ToUpper() ?? "", // Safely capitalize QIdentification, default to empty if null
                            tuple.Item2?.ToUpper() ?? "", // Safely capitalize Answer, default to empty if null
                            tuple.Item3 ?? "" // Provide default empty if Description is null
                        )).ToList();
                }

                var tpqSubmittedEvent = _mapper.Map<TargetPromotionQuestionnaireSubmittedEvent>(request);

                tpqSubmittedEvent.IsVerified = false;

                var aggregate = new TPQuestionnaireAggregate(tpqSubmittedEvent);

                await _questionnaireESH.SaveAsync(aggregate);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling SubmitTPQCommand");
                throw;
            }
        }
    }
}