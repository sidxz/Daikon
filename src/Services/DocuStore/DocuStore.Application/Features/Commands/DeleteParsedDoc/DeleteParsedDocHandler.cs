
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using DocuStore.Domain.Aggregates;
using Daikon.Events.DocuStore;

namespace DocuStore.Application.Features.Commands.DeleteParsedDoc
{
    public class DeleteParsedDocHandler : IRequestHandler<DeleteParsedDocCommand, Unit>
    {
        private readonly ILogger<DeleteParsedDocHandler> _logger;
        private readonly IEventSourcingHandler<ParsedDocAggregate> _parsedDocEventSourcingHandler;
        private readonly IMapper _mapper;

        public DeleteParsedDocHandler(ILogger<DeleteParsedDocHandler> logger,
        IEventSourcingHandler<ParsedDocAggregate> parsedDocEventSourcingHandler, IMapper mapper)
        {
            _logger = logger;
            _parsedDocEventSourcingHandler = parsedDocEventSourcingHandler;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteParsedDocCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling DeleteCommentCommand: {request}");

            try
            {
                var aggregate = await _parsedDocEventSourcingHandler.GetByAsyncId(request.Id);

                var delEvent = _mapper.Map<ParsedDocDeletedEvent>(request);

                aggregate.DeleteParsedDoc(delEvent);

                await _parsedDocEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(ParsedDocAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}